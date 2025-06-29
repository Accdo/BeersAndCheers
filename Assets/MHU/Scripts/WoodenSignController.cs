using UnityEngine;
using DG.Tweening;

public class WoodenSignController : MonoBehaviour
{
    public void RotateWoodenSign()
    {
        // DOTween 시퀀스 생성
        Sequence sequence = DOTween.Sequence();

        // 1.x값 로컬 이동
        sequence.Append(transform.DOMoveX(transform.localPosition.x - 0.6f, 0.5f).SetEase(Ease.InOutQuad));

        // 2. x축을 로컬 회전
        sequence.Append(transform.DOLocalRotate(new Vector3(transform.localEulerAngles.x -15f, transform.localEulerAngles.y, transform.localEulerAngles.z), 0.5f, RotateMode.Fast).SetEase(Ease.InOutQuad));
        
     
        // 3. z축을 +180도 로컬 회전
        sequence.Append(transform.DOLocalRotate(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + 180f), 0.5f, RotateMode.Fast).SetEase(Ease.InOutQuad));

        // 4. x값 원래로 이동
        sequence.Append(transform.DOMoveX(transform.localPosition.x, 0.5f).SetEase(Ease.InOutQuad));
    }
}