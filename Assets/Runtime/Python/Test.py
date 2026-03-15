import cv2
import numpy as np
import mediapipe as mp
from mediapipe.tasks import python
from mediapipe.tasks.python import vision

# ---------------------------------------------------------
# 1. MediaPipe Modell laden
# ---------------------------------------------------------
base_options = python.BaseOptions(
    model_asset_path=r"C:\Unity Projekte\blinking-emotion-game\Assets\Runtime\Python\face_landmarker.task"
)

options = vision.FaceLandmarkerOptions(
    base_options=base_options,
    output_face_blendshapes=True,
    output_facial_transformation_matrixes=True,
    num_faces=1
)

detector = vision.FaceLandmarker.create_from_options(options)

# ---------------------------------------------------------
# 2. Wichtige Blendshapes für Emotionserkennung
# ---------------------------------------------------------
IMPORTANT_BLENDSHAPES = [
    "mouthSmileLeft", "mouthSmileRight",
    "mouthFrownLeft", "mouthFrownRight",
    "cheekSquintLeft", "cheekSquintRight",
    "browDownLeft", "browDownRight",
    "browInnerUp",
    "noseSneerLeft", "noseSneerRight",
    "eyeWideLeft", "eyeWideRight",
    "jawOpen",
    "mouthShrugLower"
]

# ---------------------------------------------------------
# 3. Emotionserkennung basierend auf Blendshapes
# ---------------------------------------------------------

def detect_emotion(b):
    # Grundwerte
    smile = b["mouthSmileLeft"] + b["mouthSmileRight"]
    frown = b["mouthFrownLeft"] + b["mouthFrownRight"]
    brow_down = b["browDownLeft"] + b["browDownRight"]
    sneer = b["noseSneerLeft"] + b["noseSneerRight"]
    jaw = b["jawOpen"]
    eye_wide = b["eyeWideLeft"] + b["eyeWideRight"]
    brow_inner = b["browInnerUp"]
    stretch = b["mouthStretchLeft"] + b["mouthStretchRight"]

    # HAPPY
    if smile > 0.55 and frown < 0.25:
        return "happy"

    # SAD (maßgeschneidert für deine Werte)
    if (
        brow_inner > 0.45 and      # sehr stark hochgezogene innere Brauen
        eye_wide < 0.10 and        # Augen NICHT weit
        stretch < 0.10 and         # Mund NICHT gestreckt
        smile < 0.20 and           # kein Lächeln
        jaw < 0.15                 # Mund fast geschlossen
    ):
        return "sad"

    # ANGRY
    if brow_down > 0.45 or sneer > 0.28:
        return "angry"

    # FEAR
    if (
        eye_wide > 0.30 and
        0.10 < jaw < 0.25 and
        stretch > 0.22 and
        brow_inner > 0.20 and
        smile < 0.35 and
        frown < 0.25
    ):
        return "fear"

    # SURPRISED
    if (
        eye_wide > 0.36 and
        jaw > 0.28 and
        brow_inner > 0.25 and
        smile < 0.35 and
        frown < 0.25
    ):
        return "surprised"

    return "neutral"

# ---------------------------------------------------------
# 4. Overlay für Emotion im Kamerabild
# ---------------------------------------------------------
def draw_emotion_overlay(frame, emotion):
    colors = {
        "happy": (0, 255, 0),
        "sad": (0, 200, 255),
        "angry": (0, 0, 255),
        "surprised": (255, 255, 0),
        "neutral": (255, 255, 255)
    }

    color = colors.get(emotion, (255, 255, 255))
    text = f"Emotion: {emotion}"

    font = cv2.FONT_HERSHEY_SIMPLEX
    scale = 1.0
    thickness = 2
    (text_w, text_h), _ = cv2.getTextSize(text, font, scale, thickness)

    cv2.rectangle(frame, (10, 10), (20 + text_w, 30 + text_h), (0, 0, 0), -1)
    cv2.putText(frame, text, (15, 30 + text_h // 2), font, scale, color, thickness)
blink_threshold = 0.25

left_closed = False
right_closed = False

left_count = 0
right_count = 0
def update_blink(blink_value, threshold, was_closed, count):
    # Auge ist gerade geschlossen
    if blink_value > threshold:
        # Übergang: offen → geschlossen
        if not was_closed:
            count += 1
        was_closed = True
    else:
        # Auge ist offen
        was_closed = False

    return was_closed, count

# ---------------------------------------------------------
# 5. Fenster mit nur den wichtigen Blendshapes
# ---------------------------------------------------------
def draw_selected_blendshapes(blendshape_dict):
    window = np.zeros((900, 600, 3), dtype=np.uint8)

    filtered = {k: blendshape_dict[k] for k in IMPORTANT_BLENDSHAPES}
    sorted_items = sorted(filtered.items(), key=lambda x: x[1], reverse=True)

    y = 40
    for name, score in sorted_items:
        text = f"{name}: {score:.3f}"

        cv2.putText(window, text, (20, y),
                    cv2.FONT_HERSHEY_SIMPLEX, 0.6, (0, 255, 0), 2)

        bar_len = int(score * 400)
        cv2.rectangle(window, (20, y + 10), (20 + bar_len, y + 35),
                      (0, 255, 0), -1)

        y += 50

    cv2.imshow("Emotion-Blendshapes", window)

# ---------------------------------------------------------
# 6. Webcam Loop
# ---------------------------------------------------------
cap = cv2.VideoCapture(0)

while True:
    ret, frame = cap.read()
    if not ret:
        break

    rgb = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    mp_image = mp.Image(image_format=mp.ImageFormat.SRGB, data=rgb)
    result = detector.detect(mp_image)

    # FaceMesh zeichnen
    if result.face_landmarks:
        h, w, _ = frame.shape
        for lm in result.face_landmarks[0]:
            x = int(lm.x * w)
            y = int(lm.y * h)
            cv2.circle(frame, (x, y), 1, (0, 255, 0), -1)

    # Blendshapes + Emotion
    if result.face_blendshapes:
        blendshape_dict = {
            b.category_name: b.score
            for b in result.face_blendshapes[0]
        }

        emotion = detect_emotion(blendshape_dict)
        draw_emotion_overlay(frame, emotion)
        draw_selected_blendshapes(blendshape_dict)
    blink_left = blendshape_dict.get("eyeBlinkLeft", 0.0)
    blink_right = blendshape_dict.get("eyeBlinkRight", 0.0)

    left_closed, left_count = update_blink(
        blink_left, blink_threshold, left_closed, left_count
    )

    right_closed, right_count = update_blink(
        blink_right, blink_threshold, right_closed, right_count
    )
        

    # Anzeigen
    frame_flipped = cv2.flip(frame, 1)
    cv2.putText(frame, f"Left Blinks: {left_count}", (20, 80),
            cv2.FONT_HERSHEY_SIMPLEX, 0.8, (0, 255, 255), 2)

    cv2.putText(frame, f"Right Blinks: {right_count}", (20, 120),
            cv2.FONT_HERSHEY_SIMPLEX, 0.8, (0, 255, 255), 2)
    cv2.imshow("Kamera", frame)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
