import cv2
import socket
import json
import mediapipe as mp
from mediapipe.tasks import python
from mediapipe.tasks.python import vision
import os

# UDP Setup
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
UNITY_IP = "127.0.0.1"
UNITY_PORT = 5005

# Absoluten Pfad zum Modell bestimmen
model_path = os.path.join(os.path.dirname(__file__), "face_landmarker.task")

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

    if result.face_blendshapes:
        # Deine MediaPipe-Version: face_blendshapes[0] ist die Liste der Category-Objekte
        blendshape_list = result.face_blendshapes[0]

        data = [{"key": b.category_name, "value": b.score} for b in blendshape_list]
        message = json.dumps({"entries": data}).encode("utf-8")
        sock.sendto(message, (UNITY_IP, UNITY_PORT))

    cv2.imshow("MediaPipe Face Tracking", frame)
    if cv2.waitKey(1) & 0xFF == 27:
        break

cap.release()
cv2.destroyAllWindows()
