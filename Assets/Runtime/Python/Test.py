import cv2
import numpy as np
import mediapipe as mp
from mediapipe.tasks import python
from mediapipe.tasks.python import vision

# --- Modell laden ---
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

# --- Webcam starten ---
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

    # --- Blendshape-Fenster erstellen ---
    blendshape_window = np.zeros((800, 500, 3), dtype=np.uint8)

    if result.face_blendshapes:
        blendshapes = result.face_blendshapes[0]

        y = 30
        for b in blendshapes:
            name = b.category_name
            score = b.score

            cv2.putText(blendshape_window, f"{name}: {score:.2f}",
                        (10, y), cv2.FONT_HERSHEY_SIMPLEX, 0.6,
                        (0, 255, 0), 1)

            bar_len = int(score * 300)
            cv2.rectangle(blendshape_window, (10, y + 10),
                          (10 + bar_len, y + 30), (0, 255, 0), -1)

            y += 40

    # Fenster anzeigen
    cv2.imshow("Kamera", cv2.flip(frame, 1))
    cv2.imshow("Blendshapes", blendshape_window)

    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
