//youtube/Unity City : How to make Jelly Mesh in unity || Softbody(tutorial)
// * may use in future *
using UnityEngine;
public class SoftBody : MonoBehaviour
{
    public float _intensity = 1f;
    public float _mass = 1f;
    public float _stiffness = 1f;
    public float _damping = .75f;
    private Mesh _meshOriginal, _meshClone;
    private MeshRenderer _renderer;
    private SoftBodyVertex[] _sbv;
    private Vector3[] _vertexArray;
    private Vector3 target;
    private float intensity;
    void Start()
    {
        _meshOriginal = GetComponent<MeshFilter>().sharedMesh;
        _meshClone = Instantiate(_meshOriginal);
        GetComponent<MeshFilter>().sharedMesh = _meshClone;
        _renderer = GetComponent<MeshRenderer>();
        _sbv = new SoftBodyVertex[_meshClone.vertices.Length];
        for (int i = _sbv.Length - 1; i > -1; i--)
            _sbv[i] = new SoftBodyVertex(i, transform.TransformPoint(_meshClone.vertices[i]));
    }
    void FixedUpdate()
    {
        _vertexArray = _meshOriginal.vertices;
        for (int i = _sbv.Length - 1; i > -1; i--)
        {
            target = transform.TransformPoint(_vertexArray[_sbv[i].ID]);
            intensity = ((1 - _renderer.bounds.max.y - target.y) / _renderer.bounds.size.y) * _intensity;
            _sbv[i].Shake(target, _mass, _stiffness, _damping);
            target = transform.InverseTransformPoint(_sbv[i].Position);
            _vertexArray[_sbv[i].ID] = Vector3.Lerp(_vertexArray[_sbv[i].ID], target, intensity);
        }
        _meshClone.vertices = _vertexArray;
    }
    protected class SoftBodyVertex
    {
        public int ID;
        public Vector3 Position;
        public Vector3 Velocity, Force;
        public SoftBodyVertex(int id, Vector3 position)
        {
            ID = id;
            Position = position;
        }
        public void Shake(Vector3 target, float mass, float stiffness, float damping)
        {
            Force = (target - Position) * stiffness;
            Velocity = (Velocity + Force / mass) * damping;
            Position += Velocity;
            if ((Velocity + Force + Force / mass).magnitude < .001f)
                Position = target;
        }
    }
}
