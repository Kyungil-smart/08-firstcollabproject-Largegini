using UnityEngine;

// 요약: 부모를 임의로 변경해주는 스크립트
// 작성자: 이성규
public class SetParent : MonoBehaviour
{
    [SerializeField] private Transform _partnt;

    private void Start()
    {
        // 게임 시작 시, 오브젝트의 부모를 변경
        transform.SetParent(_partnt);
        //부모 오브젝트 위치에 맞도록 로컬 포지션 제로
        GetComponent<Transform>().localPosition = Vector3.zero;
    }
}