import torch
import torch.nn as nn
import torch.nn.functional as F

class ResidualBlock(nn.Module):
    def __init__(self, channels):
        super().__init__()
        self.feature = nn.Sequential(
            nn.Conv2d(channels, channels, 3, padding=1),
            nn.BatchNorm2d(channels),
            nn.ReLU(),
            nn.Conv2d(channels, channels, 3, padding=1),
            nn.BatchNorm2d(channels),
        )
    def forward(self, x):
        r = self.feature(x)
        return F.relu(x + r)



class NoGoModel(nn.Module):
    def __init__(self):
        super(NoGoModel, self).__init__()
        self.stem = nn.Sequential(
            nn.Conv2d(4, 128, kernel_size=3, padding=1),
            nn.BatchNorm2d(128),
            nn.ReLU(),
            )
        self.resblocks = nn.Sequential(
            *[ResidualBlock(128) for _ in range(0, 6)]
        )
        self.policy_head = nn.Sequential(
            nn.Conv2d(128, 128, 3, padding=1),
            nn.ReLU(),
            nn.Flatten(),
            nn.Linear(10368, 128),
            nn.ReLU(),
            nn.Linear(128, 81),
            nn.Softmax()
        )
        self.value_head = nn.Sequential(
            nn.Conv2d(128, 1,1),
            nn.Flatten(),
            nn.Linear(81, 128),
            nn.ReLU(),
            nn.Linear(128,1),
            nn.Tanh()
            )
    def forward(self, x):
        x = self.stem(x)
        x = self.resblocks(x)
        policy = self.policy_head(x)
        value = self.value_head(x)
        return policy, value
