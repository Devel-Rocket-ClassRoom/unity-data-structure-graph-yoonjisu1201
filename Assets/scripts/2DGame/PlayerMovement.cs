
using System.Collections;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    private Stage stage;
    private Animator animator;
    public int currentTileId = -1;
    public int targetTileId = -1;
    public float moveSpeed = 10f;

    private bool isMoving = false;
    private Coroutine coMove = null;
    private Coroutine coMovePath = null;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0;

        var findGo = GameObject.FindWithTag("Map");
        stage = findGo.GetComponent<Stage>();
    }
    private void Update()
    {

        //var direction = Sides.None;
        //if (Input.GetKeyDown(KeyCode.W)) direction = Sides.Top;
        //else if (Input.GetKeyDown(KeyCode.S)) direction = Sides.Bottom;
        //else if (Input.GetKeyDown(KeyCode.D)) direction = Sides.Right;
        //else if (Input.GetKeyDown(KeyCode.A)) direction = Sides.Left;
        //if (direction != Sides.None)
        //{
        //    var targetTile = stage.Map.tiles[currentTileId].adjacents[(int)direction];
        //    if (targetTile != null && targetTile.CanMove)
        //    {
        //        MoveTo(targetTile.id);
        //        animator.SetBool("isMoving", true);
        //    }
        //}
        //else
        //{
        //    animator.SetBool("isMoving", false);
        //}
        if (Input.GetMouseButtonDown(0))
        {
            var clickTileId = stage.ScreenPosToTileId(Input.mousePosition);
            var targetTile = stage.Map.tiles[clickTileId];
            if (targetTile != null && targetTile.CanMove)
            {
                MoveTo(targetTile.id);
            }
        }
    }

    public void MoveTo(int tileId)
    {
        if (isMoving) return;

        targetTileId = tileId;

        if (coMove != null)
        {
            StopCoroutine(coMove);
            coMove = null;
        }
        coMove = StartCoroutine(CoMove());
    }

    private IEnumerator CoMove()
    {
        isMoving = true;
        animator.speed = 1f;
        int currentTargetTileId = targetTileId;
        var path = stage.Map.PathFindingAStar(currentTileId, currentTargetTileId);
        if (path.Count == 0)
        {
            isMoving = false;
            animator.speed = 0f;
            coMove = null;
            yield break;
        }
        stage.DrowPath(path);
        var pathIndex = 1;
        while (pathIndex < path.Count)
        {
            if (currentTargetTileId != targetTileId)
            {
                currentTargetTileId = targetTileId;
                path = stage.Map.PathFindingAStar(currentTileId, currentTargetTileId);
                if (path.Count == 0)
                {
                    isMoving = false;
                    animator.speed = 0f;
                    coMove = null;
                    yield break;
                }
                stage.DrowPath(path);
                pathIndex = 1;
            }
            var startPos = transform.position;
            var endPos = stage.GetTilePos(path[pathIndex].id);
            var duration = Vector3.Distance(startPos, endPos) / moveSpeed;
            var t = 0f;

            while (t < 1f)
            {
                t += Time.deltaTime / duration;
                transform.position = Vector3.Lerp(startPos, endPos, t);
                yield return null;
            }
            transform.position = endPos;
            currentTileId = path[pathIndex].id;
            stage.OnTileVisited(currentTileId);
            ++pathIndex;
        }

        animator.speed = 0f;
        isMoving = false;
        coMove = null;
    }

    public void WarpTo(int tileId)
    {
        if (coMove != null)
        {
            StopCoroutine(coMove);
            coMove = null;
        }
        isMoving = false;
        targetTileId = -1;

        currentTileId = tileId;
        transform.position = stage.GetTilePos(tileId);

        Debug.Log($"WarpTo()");

        stage.OnTileVisited(stage.Map.tiles[currentTileId]);
        stage.DecorateAllTile();   // 타일 업데이트
    }
}
