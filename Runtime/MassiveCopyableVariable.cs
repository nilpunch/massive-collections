using System.Runtime.CompilerServices;

namespace Massive.Collections
{
	public class MassiveCopyableVariable<T> : IMassive where T : ICopyable<T>
	{
		private readonly CyclicFrameCounter _cyclicFrameCounter;
		private readonly T[] _valueByFrames;
		private T _value;

		public MassiveCopyableVariable(T initialValue = default, int framesCapacity = Constants.DefaultFramesCapacity)
		{
			_cyclicFrameCounter = new CyclicFrameCounter(framesCapacity);
			_valueByFrames = new T[framesCapacity];
			Value = initialValue;
		}

		public ref T Value
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => ref _value;
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

			Value.CopyTo(ref _valueByFrames[_cyclicFrameCounter.CurrentFrame]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Rollback(int frames)
		{
			_cyclicFrameCounter.Rollback(frames);

			_valueByFrames[_cyclicFrameCounter.CurrentFrame].CopyTo(ref Value);
		}
	}
}
