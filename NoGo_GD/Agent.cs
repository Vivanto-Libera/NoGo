using Godot;
using NoGo;
using NoGO;
using System.Threading;
using static TorchSharp.torch;

namespace NoGo
{
	public partial class Agent() : Node
	{
		[Signal]
		public delegate void AiSelectedMoveEventHandler(int moveIndex);

		private Thread aiThread;
		private CancellationTokenSource cts;
		private GameBoard board;
		private readonly NoGoModel model = new();
		public int sims = 200;

		public void StartThread()
		{
			cts = new CancellationTokenSource();
			CancellationToken token = cts.Token;
			aiThread = new Thread(() => SelectMove(token));
			aiThread.Start();
		}
		public void StopThread()
		{
			if (aiThread != null && aiThread.IsAlive)
			{
				cts?.Cancel();
				aiThread.Join(1000);
				cts.Dispose();
				cts = null;
			}
		}
		public void SetBoard(GameBoard board)
		{
			this.board = board;
		}

		private void SelectMove(CancellationToken token) 
		{
			MCEdge rootEdge = new(null, null)
			{
				N = 1
			};
			MCNode rootNode = new(new(board), rootEdge);
			MCTS mctsSearcher = new(model);
			mctsSearcher.SetSims(sims);
			float[] moveProb = mctsSearcher.Search(rootNode, token);
			if (token.IsCancellationRequested)
			{
				return;
			}
			Tensor randMove = multinomial(tensor(moveProb), 1);
			CallDeferred(nameof(EmitMove), randMove.item<long>());
		}

		private void EmitMove(int moveIndex)
		{
			EmitSignal(SignalName.AiSelectedMove, moveIndex);
		}
	}
}
