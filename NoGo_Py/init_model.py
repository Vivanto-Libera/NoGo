import torch
import torch.nn as nn
from nogoModel import NoGoModel

model = NoGoModel()
torch.save(model.state_dict(), "init_model.pt")
