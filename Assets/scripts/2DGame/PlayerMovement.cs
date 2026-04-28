using System.Net.NetworkInformation;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerMovement : MonoBehaviour
{
    private Stage stage;
    private Animator animator;
    private int currentTileId;
    private bool isMoving = false;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        animator.speed = 0;

        var findGo = GameObject.FindWithTag("Map");
        stage = findGo.GetComponent<Stage>();
    }
    private void Update()
    {

        var direction = Sides.None;
        if (Input.GetKeyDown(KeyCode.W)) direction = Sides.Top;
        else if (Input.GetKeyDown(KeyCode.S)) direction = Sides.Bottom;
        else if (Input.GetKeyDown(KeyCode.D)) direction = Sides.Right;
        else if (Input.GetKeyDown(KeyCode.A)) direction = Sides.Left;
        if (direction != Sides.None)
        {
            var targetTile = stage.Map.tiles[currentTileId].adjacents[(int)direction];
            if (targetTile != null && targetTile.CanMove)
            {
                MoveTo(targetTile.id);
                animator.SetBool("isMoving", true);
            }
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }
    public void MoveTo(int tileId)
    {
        currentTileId = tileId;
        transform.position = stage.GetTilePos(currentTileId);

        stage.UpdateVision(currentTileId);
    }
    public void MoveTo(int x, int y)
    {

    }
}
