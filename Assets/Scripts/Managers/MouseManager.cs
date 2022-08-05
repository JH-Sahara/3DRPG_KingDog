using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// [System.Serializable]
// public class EventVector3 : UnityEvent<Vector3> {}
public class MouseManager : Singleton<MouseManager>
{
    public Texture2D point,doorway,attack,target,arrow;
    public Action<Vector3> onMouseClicked;
    public Action<GameObject> onEnemyClicked;
    public RaycastHit hitInfo;

    override protected void Awake() {
        base.Awake();
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        SetCursorTextrue();
        MouseControl();
    }

    //获取射线碰撞信息并修改光标texture
    private void SetCursorTextrue()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray,out hitInfo))
        {
            //切换鼠标texture
            switch (hitInfo.collider.gameObject.tag)
            {
                case "Ground" :
                    Cursor.SetCursor(target,new Vector2(16,16),CursorMode.Auto);
                    break;
                case "Enemy" :
                    Cursor.SetCursor(attack,new Vector2(16,16),CursorMode.Auto);
                    break;
                case "Attack" :
                    Cursor.SetCursor(attack,new Vector2(16,16),CursorMode.Auto);
                    break;
                case "Portal" :
                    Cursor.SetCursor(doorway,new Vector2(16,16),CursorMode.Auto);
                    break;
                default:
                    Cursor.SetCursor(arrow,new Vector2(0,0),CursorMode.Auto);
                    break;
            }
        }
    }

    //控制event事件触发
    private void MouseControl()
    {
        if (Input.GetMouseButtonDown(0) && hitInfo.collider!=null)
        {
            if (hitInfo.collider.gameObject.CompareTag("Ground"))
            {
                onMouseClicked?.Invoke(hitInfo.point);
            }
            if (hitInfo.collider.gameObject.CompareTag("Enemy") || hitInfo.collider.CompareTag("Attack"))
            {
                onEnemyClicked?.Invoke(hitInfo.collider.gameObject);
            }
            if (hitInfo.collider.gameObject.CompareTag("Portal"))
            {
                onMouseClicked?.Invoke(hitInfo.point);
            }
        }
    }
}
