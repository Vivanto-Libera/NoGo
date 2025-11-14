import torch
from torch.utils.data import Dataset
class NoGoDataset(Dataset):
    def __init__(self, pos, probs, value):
        self.pos = torch.tensor(pos, dtype=torch.float32)
        self.probs = torch.tensor(probs, dtype=torch.float32)
        self.value = torch.tensor(value, dtype=torch.float32)
    def __len__(self):
        return len(self.pos)
    def __getitem__(self, idx):
        return self.pos[idx], self.probs[idx], self.value[idx]
