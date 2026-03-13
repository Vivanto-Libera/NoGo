import mcts
import torch
from tqdm import tqdm
import numpy as np
from dataset import NoGoDataset
from nogoModel import NoGoModel
import torch.optim as optim
from torch.utils.data import Dataset, DataLoader
import torch.nn as nn
import os
import time


model = NoGoModel()
i = 0
for k in range(0, 100000000000):
    if not os.path.exists("model_it"+str(k)+".pt"):
        i = k - 1
        break
model.load_state_dict(torch.load("model_it"+str(i)+".pt"))
policy_loss = nn.CrossEntropyLoss()
value_loss = nn.MSELoss()
optimizer = optim.Adam(model.parameters(), lr=1e-4)
threads = 16
while True:
    expSaved = True
    for j in range(0, threads):
        if not os.path.exists(f'exp_tr{j}_it{i}.npz'):
            expSaved = False
            break
    if expSaved:
        time.sleep(5)
        allPos = np.empty(0)
        allProbs = np.empty(0)
        allValues = np.empty(0)
        for j in range(0, threads):
            data = np.load(f'exp_tr{j}_it{i}.npz')
            if j == 0:
                allPos = data['pos']
                allProbs = data['probs']
                allValues = data['values']
            else:
                allPos = np.concatenate([data['pos'], allPos], axis=0)
                allProbs = np.concatenate([data['probs'], allProbs], axis=0)
                allValues = np.concatenate([data['values'], allValues], axis=0)
        train_dataset = NoGoDataset(allPos, allProbs, allValues)
        train_loader = DataLoader(train_dataset, batch_size=256, shuffle=True)
        device= torch.device("cuda" if torch.cuda.is_available() else "cpu")
        model = model.to(device)
        model.train()
        for epoch in range(0,1):
            for batch_pos, batch_probs,batch_val in train_loader:
                batch_pos = batch_pos.to(device)
                batch_probs = batch_probs.to(device)
                batch_val = batch_val.to(device)
                optimizer.zero_grad()
                pred_policy, pred_value = model(batch_pos)
                loss_policy = policy_loss(pred_policy, batch_probs)
                loss_value = value_loss(pred_value, batch_val)
                loss = loss_policy + loss_value
                loss.backward()
                optimizer.step()
        i += 1
        torch.save(model.state_dict(), "model_it"+str(i)+".pt")
        print("model saved")
        allPos = None
        allProbs = None
        allValues = None
        train_dataset = None
        train_loader = None
    else:
        time.sleep(60)
