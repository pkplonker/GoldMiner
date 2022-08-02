using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace UI
{
	public class LoadingDots : MonoBehaviour
	{
		//the total time of the animation
		public float _repeatTime = 1;

		//the time for a dot to bounce up and come back down
		public float _bounceTime = 0.25f;

		//how far does each dot move
		public float _bounceHeight = 10;

		public List<GameObject> _dots;

		private void Start()
		{
			if (_repeatTime < _dots.Count * _bounceTime)
			{
				_repeatTime = _dots.Count * _bounceTime;
			}

			InvokeRepeating(nameof(Animate), 0, _repeatTime);
		}

		private void Animate()
		{
			for (var i = 0; i < _dots.Count; i++)
			{
				var dotIndex = i;

				_dots[dotIndex].transform
					.DOMoveY(_dots[dotIndex].transform.position.y + _bounceHeight, _bounceTime / 2)
					.SetDelay(dotIndex * _bounceTime / 2)
					.SetEase(Ease.OutQuad)
					.OnComplete(() =>
					{
						_dots[dotIndex].transform
							.DOMoveY(_dots[dotIndex].transform.position.y - _bounceHeight, _bounceTime / 2)
							.SetEase(Ease.InQuad);
					});
			}
		}
	}
}