import torch
import torch.onnx
from nogoModel import NoGoModel
from NoGo import Board
import numpy as np
model = NoGoModel()
model.load_state_dict(torch.load("new_model.pt",map_location='cpu'))
model.eval()
board = Board()
input = torch.FloatTensor(board.neuralworkInput()).unsqueeze(0)
traced_model = torch.jit.trace(model, input)
traced_model.save("Baduk.pt")
