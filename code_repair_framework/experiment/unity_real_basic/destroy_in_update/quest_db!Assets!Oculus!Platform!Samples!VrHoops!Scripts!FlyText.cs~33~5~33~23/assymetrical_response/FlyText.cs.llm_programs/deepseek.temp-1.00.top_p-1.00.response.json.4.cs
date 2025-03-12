namespace Oculus.Platform.Samples.VrHoops
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;


	public class FlyText : MonoBehaviour
	{

		private const float LIFESPAN = 3.0f;


		private readonly Vector3 m_movePerFrame = 0.5f * Vector3.up;


		private float m_eol;

		void Start()
		{
			m_eol = Time.time + LIFESPAN;
			GetComponent<Text>().CrossFadeColor(Color.black, LIFESPAN * 1.7f, false, true);
		}




在这个脚本中，我们正在实现一个功能，即允许文本对象 (UnityEngine.UI.Text) 在屏幕上持久存在一段时间后逐渐淡出。这是一个常见的在游戏中展示信息的方式。

在 `Start` 函数中，我们将文本的生命周期 (m_eol, time of life) 设置为当前时间加上预定义的常量 `LIFESPAN`，即文本存在的总时间。

然后，我们在 `Update` 函数中检查当前时间是否大于 `m_eol`。如果是，我们使用 `CrossFadeColor` 方法淡出文本，参数为淡出的目标颜色 (Color.black)、淡出持续时间、是否稳定 (false) 和淡出后保持最后一个颜色 (true)。


	}
}
