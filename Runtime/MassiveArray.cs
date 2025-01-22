using System;
using System.Runtime.CompilerServices;

namespace Massive.Collections
{
	public class MassiveList<T> : FastList<T>, IMassive
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;
		private readonly T[][] _itemsByFrames;
		private readonly int[] _countByFrames;

		public MassiveList(int framesCapacity = Constants.DefaultFramesCapacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);
			_itemsByFrames = new T[framesCapacity][];
			_countByFrames = new int[framesCapacity];

			for (var i = 0; i < framesCapacity; i++)
			{
				_itemsByFrames[i] = Array.Empty<T>();
			}
		}

		public int CanRollbackFrames
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _cyclicFrameCounter.CanRollbackFrames;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SaveFrame()
		{
			_cyclicFrameCounter.SaveFrame();

			var currentFrame = _cyclicFrameCounter.CurrentFrame;

			EnsureCapacityForFrame(currentFrame);

			// Copy everything from current state to current frame.
			Array.Copy(Items, _itemsByFrames[currentFrame], Count);
			_countByFrames[currentFrame] = Count;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			// Copy everything from rollback frame to current state.
			var rollbackFrame = _cyclicFrameCounter.CurrentFrame;
			var rollbackCount = _countByFrames[rollbackFrame];

			Array.Copy(_itemsByFrames[rollbackFrame], Items, rollbackCount);
			Count = rollbackCount;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void EnsureCapacityForFrame(int frame)
		{
			if (_itemsByFrames[frame].Length < Count)
			{
				Array.Resize(ref _itemsByFrames[frame], Capacity);
			}
		}
	}
}
