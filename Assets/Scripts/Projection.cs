using UnityEngine;
using UnityEngine.SceneManagement;

public class Projection : MonoBehaviour
{
    private Scene _simulationScene;
    private PhysicsScene2D _physicsScene;

    public Transform obstaclesParent;

    public LineRenderer line;
    public int maxPhysicsFrameIteration = 100;

    private GameObject bullet;

    private void Start()
    {
        CreatePysicsScene();
    }

    // Start is called before the first frame update
    void CreatePysicsScene()
    {
        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
        _physicsScene = _simulationScene.GetPhysicsScene2D();
    }

    private void FixedUpdate()
    {
    }

    public void UpdatePysicsScene()
    {
        foreach(GameObject obj in _simulationScene.GetRootGameObjects())
        {
            Destroy(obj);
        }

        foreach (Transform obj in obstaclesParent)
        {
            var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
            ghostObj.GetComponent<Renderer>().enabled = false;
            foreach (Transform child in ghostObj.transform)
            {
                if (child.GetComponent<Renderer>() != null)
                {
                    child.GetComponent<Renderer>().enabled = false;
                }
                
                if (child.GetComponent<Bonus>() != null)
                {
                    Destroy(child.gameObject);
                }
                
            }
            SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
            
        }
    }

   public void SimulateTrajectory(GameObject bulletPrefab, Vector2 pos, Vector2 velocity)
   {
        var bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(bullet.gameObject, _simulationScene);
        bullet.GetComponent<Bullet>().changeMaterial(Bullet.materialsType.BOUNCE);
        bullet.GetComponent<Bullet>().isGhost = true;
        bullet.layer = 8;

        bullet.GetComponent<Bullet>().Shot(velocity, 1);

        line.positionCount = maxPhysicsFrameIteration;

        for (int i = 0; i < maxPhysicsFrameIteration; i++)
        {
            _physicsScene.Simulate(Time.fixedDeltaTime);
            line.SetPosition(i, bullet.transform.position);
        }

        Destroy(bullet.gameObject);
   }
}
