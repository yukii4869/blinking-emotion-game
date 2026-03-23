import cv2
import socket
import json
import mediapipe as mp
from mediapipe.tasks import python
from mediapipe.tasks.python import vision
import os
import numpy as np

# UDP Setup
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
UNITY_IP = "127.0.0.1"
UNITY_PORT = 5005

# Absoluten Pfad zum Modell bestimmen
model_path = os.path.join(os.path.dirname(__file__), "face_landmarker.task")

LEFT_EYE = [33, 159, 158, 133, 153, 145]
RIGHT_EYE = [362, 386, 387, 263, 374, 380]

def dist(a, b):
    return np.linalg.norm(np.array(a) - np.array(b))

def compute_ear(landmarks, idx):
    p = [landmarks[i] for i in idx]
    vert1 = dist((p[1]["x"], p[1]["y"]), (p[5]["x"], p[5]["y"]))
    vert2 = dist((p[2]["x"], p[2]["y"]), (p[4]["x"], p[4]["y"]))
    horiz = dist((p[0]["x"], p[0]["y"]), (p[3]["x"], p[3]["y"]))
    return (vert1 + vert2) / (2.0 * horiz)

def draw_eye_points(frame, landmarks, idx, color):
    h, w, _ = frame.shape
    for i in idx:
        x = int(landmarks[i]["x"] * w)
        y = int(landmarks[i]["y"] * h)
        cv2.circle(frame, (x, y), 3, color, -1)

# Model laden
base_options = python.BaseOptions(
    model_asset_path=model_path
)

options = vision.FaceLandmarkerOptions(
    base_options=base_options,
    output_face_blendshapes=True,
    output_facial_transformation_matrixes=True,
    num_faces=1
)

detector = vision.FaceLandmarker.create_from_options(options)

# Webcam öffnen
cap = cv2.VideoCapture(0)

while True:
    ret, frame = cap.read()
    if not ret:
        continue

    mp_image = mp.Image(image_format=mp.ImageFormat.SRGB, data=frame)
    result = detector.detect(mp_image)

    # Immer initialisieren!
    landmarks = None
    blendshapes = None
    
    # Landmarks extrahieren
    if result.face_landmarks:
        lm = result.face_landmarks[0]
        landmarks = [{"x": p.x, "y": p.y, "z": p.z} for p in lm]
        # EAR berechnen
        left_ear = compute_ear(landmarks, LEFT_EYE)
        right_ear = compute_ear(landmarks, RIGHT_EYE)
        ear = (left_ear + right_ear) / 2.0

        # Punkte zeichnen
        draw_eye_points(frame, landmarks, LEFT_EYE, (0, 255, 0))
        draw_eye_points(frame, landmarks, RIGHT_EYE, (255, 0, 0))

        # EAR anzeigen
        cv2.putText(frame, f"EAR: {ear:.3f}", (30, 50),
                    cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0), 2)

    # Blendshapes extrahieren
    if result.face_blendshapes:
        blendshape_list = result.face_blendshapes[0]
        blendshapes = [{"key": b.category_name, "value": b.score} for b in blendshape_list]

    # Nur senden, wenn mindestens etwas vorhanden ist
    if landmarks is not None or blendshapes is not None:
        message = json.dumps({
            "landmarks": landmarks,
            "blendshapes": blendshapes
        }).encode("utf-8")

        sock.sendto(message, (UNITY_IP, UNITY_PORT))

    

    cv2.imshow("MediaPipe Face Tracking", frame)
    if cv2.waitKey(1) & 0xFF == 27:
        break

cap.release()
cv2.destroyAllWindows()
try:
    sock.close()
except:
    pass

try:
    del detector
except:
    pass

import sys
sys.exit(0)
