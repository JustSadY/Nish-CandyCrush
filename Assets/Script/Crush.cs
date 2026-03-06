using System.Collections;
using UnityEngine;

public class Crush : MonoBehaviour
{
    public int xIndex;
    public int yIndex;
    [SerializeField] private CrushType _crushType;
    private bool bIsMatched = false;
    private Vector2 _currentPosition;
    private Vector2 _targetPosition;
    private bool _bIsMoving = false;

    public void InitializeCrush(int xIndex, int yIndex)
    {
        this.xIndex = xIndex;
        this.yIndex = yIndex;
    }

    public void MoveToTarget(Vector2 targetPosition)
    {
        StartCoroutine(MoveCoroutine(targetPosition));
    }

    private IEnumerator MoveCoroutine(Vector2 targetPosition)
    {
        _bIsMoving = true;
        float duration = 0.2f;

        Vector2 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
        _bIsMoving = false;
    }

    public bool IsMatched() => bIsMatched;
    public void SetMatched(bool bIsMatched) => this.bIsMatched = bIsMatched;
    public CrushType GetCrushType() => _crushType;
}