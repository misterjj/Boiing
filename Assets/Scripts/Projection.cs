using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projection : MonoBehaviour
{
    private Scene _mainScene;
    private PhysicsScene _physicsMainScene;
    private Scene _simulationScene;
    private PhysicsScene _physicsScene;

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
        Physics.autoSimulation = false;
        _mainScene = SceneManager.CreateScene("MainScene");
        _physicsMainScene = _mainScene.GetPhysicsScene();
        _simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics3D));
        _physicsScene = _simulationScene.GetPhysicsScene();
    }

    private void FixedUpdate()
    {
        if (!_physicsMainScene.IsValid())
        {
            return;
        }

        _physicsMainScene.Simulate(Time.fixedDeltaTime);
    }

    public void UpdatePysicsScene()
    {
        //foreach(GameObject obj in _simulationScene.GetRootGameObjects())
        //{
        //    Destroy(obj);
        //}

        //foreach (Transform obj in obstaclesParent)
        //{
        //    var ghostObj = Instantiate(obj.gameObject, obj.position, obj.rotation);
        //    ghostObj.GetComponent<Renderer>().enabled = false;
        //    foreach (Transform child in ghostObj.transform)
        //    {
        //        if (child.GetComponent<Renderer>() != null)
        //        {
        //            child.GetComponent<Renderer>().enabled = false;
        //        }
                
        //    }
        //    SceneManager.MoveGameObjectToScene(ghostObj, _simulationScene);
            
        //}
    }

   public void SimulateTrajectory(GameObject bulletPrefab, Vector2 pos, Vector2 velocity)
   {
        var bullet = Instantiate(bulletPrefab, pos, Quaternion.identity);
        SceneManager.MoveGameObjectToScene(bullet.gameObject, _simulationScene);
        bullet.GetComponent<Bullet>().changeMaterial(Bullet.materialsType.BOUNCE);
        bullet.name = "toto";

        _physicsScene.Simulate(Time.fixedDeltaTime);
        bullet.GetComponent<Bullet>().Shot(velocity);

        line.positionCount = maxPhysicsFrameIteration;

        for (int i = 0; i < maxPhysicsFrameIteration; i++)
        {
            Debug.Log(Time.fixedDeltaTime);
            _physicsScene.Simulate(Time.fixedDeltaTime);
            line.SetPosition(i, bullet.transform.position);
            Debug.Log(bullet.transform.position);
        }

        Destroy(bullet.gameObject, 1.0f);
   }
}
