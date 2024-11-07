using UnityEngine;

public class CubeBossAnimReceiver : MonoBehaviour
{
    [SerializeField] private IngameSceneController sc;

    public void Test(string message)
    {
        Debug.Log(message);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Vector3 normal = contact.normal;
                FaceDirection contactFace = GetContactFace(normal);
                Debug.Log("PlayerがCubeBossの" + contactFace + "面に接触しました: " + collision.gameObject.name);
                if (contactFace == FaceDirection.Down)
                {
                    sc.GivePlayerDamage(10000);
                }
            }
        }
    }

    private FaceDirection GetContactFace(Vector3 normal)
    {
        if (Vector3.Dot(normal, Vector3.up) > 0.5f)
        {
            return FaceDirection.Down;
        }
        else if (Vector3.Dot(normal, Vector3.down) > 0.5f)
        {
            return FaceDirection.Up;
        }
        else if (Vector3.Dot(normal, Vector3.left) > 0.5f)
        {
            return FaceDirection.Left;
        }
        else if (Vector3.Dot(normal, Vector3.right) > 0.5f)
        {
            return FaceDirection.Right;
        }
        else if (Vector3.Dot(normal, Vector3.forward) > 0.5f)
        {
            return FaceDirection.Forward;
        }
        else if (Vector3.Dot(normal, Vector3.back) > 0.5f)
        {
            return FaceDirection.Back;
        }
        else
        {
            return FaceDirection.Unknown;
        }
    }

    private enum FaceDirection
    {
        Up,
        Down,
        Left,
        Right,
        Forward,
        Back,
        Unknown
    }
}