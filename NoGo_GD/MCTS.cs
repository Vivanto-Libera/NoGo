using Microsoft.VisualBasic;
using NoGO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TorchSharp;

namespace NoGo
{
    public struct EdgeNode
    {
        public MCEdge edge;
        public MCNode node;
        public EdgeNode(MCEdge edge, MCNode node)
        {
            this.edge = edge;
            this.node = node;
        }
    }
    public class MCEdge
    {
        public int? move;
        public int N = 0;
        public float W = 0;
        public float Q = 0;
        public float P = 0;
#nullable enable
        public MCNode? parentNode;

        public MCEdge(int? move, MCNode? parentNode)
        {
            this.move = move;
            this.parentNode = parentNode;
        }
    }
    public class MCNode
    {
        public GameBoard board;
        public MCEdge parentEdge = new(null, null);
        public List<EdgeNode> childEdgeNodes = [];
        public float Expand(NoGoModel model)
        {
            List<int> moves = board.LegalMoves();
            foreach (int move in moves)
            {
                GameBoard childBoard = new(board);
                childBoard.ApplyMove(move);
                MCEdge childEdge = new(move, this);
                MCNode childNode = new(childBoard, childEdge);
                childEdgeNodes.Add(new EdgeNode(childEdge, childNode));
            }
            var (prob, value) = model.Predict(board.NetWorkInput());
            float probSum = 0;
            foreach (EdgeNode edgeNode in childEdgeNodes)
            {
                edgeNode.edge.P = prob[0, edgeNode.edge.move].item<float>();
                probSum += edgeNode.edge.P;
            }
            if (probSum > 0)
            {
                foreach (EdgeNode edgeNode in childEdgeNodes)
                {
                    edgeNode.edge.P /= probSum;
                }
            }
            return value[0, 0].item<float>();
        }
        public bool IsLeaf()
        {
            return childEdgeNodes.Count == 0;
        }
        public MCNode(GameBoard board, MCEdge parentEdge)
        {
            this.board = board;
            this.parentEdge = parentEdge;
        }
    }
    public class MCTS
    {
        private NoGoModel model;
#nullable enable
        private MCNode? rootNode = null;
        private float tau = 0.1f;
        private float cPuct = 1.5f;
        private int sims = 100;
        public MCTS(NoGoModel model)
        {
            this.model = model;
        }
        public void SetSims(int sims)
        {
            this.sims = sims;
        }
        public float[] Search(MCNode rootNode, CancellationToken token)
        {
            this.rootNode = rootNode;
            this.rootNode.Expand(model);
            if (token.IsCancellationRequested)
            {
                return [0.0f];
            }
            for (int i = 0; i < sims; i++)
            {
                if (token.IsCancellationRequested)
                {
                    return [0.0f];
                }
                MCNode selectedNode = Select(rootNode);
                ExpandAndEvaluate(selectedNode);
            }
            if (token.IsCancellationRequested)
            {
                return [0.0f];
            }
            int NSum = 0;
            float[] moveProbs = new float[81];
            foreach (EdgeNode edgeNode in rootNode.childEdgeNodes)
            {
                if (token.IsCancellationRequested)
                {
                    return [0.0f];
                }
                NSum += edgeNode.edge.N;
            }
            foreach (EdgeNode edgeNode in rootNode.childEdgeNodes)
            {
                if (token.IsCancellationRequested)
                {
                    return [0.0f];
                }
                float prob = ((float)Math.Pow(edgeNode.edge.N, (1 / tau))) / (float)Math.Pow(NSum, (1 / tau));
                moveProbs[edgeNode.edge.move.Value] = prob;
            }
            return moveProbs;
        }
        private float UctValues(MCEdge edge, int ParentN)
        {
            return cPuct * edge.P * ((float)Math.Sqrt(ParentN) / (1 + edge.N));
        }
        private MCNode Select(MCNode node)
        {
            if (node.IsLeaf())
            {
                return node;
            }
            else
            {
                MCNode? maxUctChild = null;
                float maxUctValue = -100000000000;
                foreach (EdgeNode edgeNode in node.childEdgeNodes)
                {
                    float uctValues = UctValues(edgeNode.edge, edgeNode.edge.parentNode.parentEdge.N);
                    float val = edgeNode.edge.Q;
                    if (edgeNode.edge.parentNode.board.turn == StoneColor.BLACK)
                    {
                        val = -val;
                    }
                    float uctValChild = val + uctValues;
                    if (uctValChild > maxUctValue)
                    {
                        maxUctValue = uctValChild;
                        maxUctChild = edgeNode.node;
                    }
                }
                List<MCNode> allBestChild = [];
                return Select(maxUctChild);
            }
        }
        private void BackUp(float value, MCEdge edge)
        {
            edge.N += 1;
            edge.W += value;
            edge.Q = edge.W / edge.N;
            if (edge.parentNode != null)
            {
                if (edge.parentNode.parentEdge != null)
                {
                    BackUp(value, edge.parentNode.parentEdge);
                }
            }
        }
        private void ExpandAndEvaluate(MCNode node)
        {
            StoneColor winner = node.board.IsTerminal();
            float v = 0;
            if (winner != StoneColor.EMPTY)
            {
                if (winner == StoneColor.WHITE)
                {
                    v = 1;
                }
                else if (winner == StoneColor.BLACK)
                {
                    v = -1;
                }
                BackUp(v, node.parentEdge);
                return;
            }
            v = node.Expand(model);
            BackUp(v, node.parentEdge);
        }
    }
}
