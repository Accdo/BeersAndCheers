using UnityEngine;
using DG.Tweening;

public class WoodenSignController : MonoBehaviour
{
    public void RotateWoodenSign()
    {
        // DOTween ������ ����
        Sequence sequence = DOTween.Sequence();

        // 1.x�� ���� �̵�
        sequence.Append(transform.DOMoveX(transform.localPosition.x - 0.6f, 0.5f).SetEase(Ease.InOutQuad));

        // 2. x���� ���� ȸ��
        sequence.Append(transform.DOLocalRotate(new Vector3(transform.localEulerAngles.x -15f, transform.localEulerAngles.y, transform.localEulerAngles.z), 0.5f, RotateMode.Fast).SetEase(Ease.InOutQuad));
        
     
        // 3. z���� +180�� ���� ȸ��
        sequence.Append(transform.DOLocalRotate(new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + 180f), 0.5f, RotateMode.Fast).SetEase(Ease.InOutQuad));

        // 4. x�� ������ �̵�
        sequence.Append(transform.DOMoveX(transform.localPosition.x, 0.5f).SetEase(Ease.InOutQuad));
    }
}