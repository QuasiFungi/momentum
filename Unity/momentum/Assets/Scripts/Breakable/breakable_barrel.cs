using UnityEngine;
public class breakable_barrel : base_breakable
{
    [SerializeField] protected GameObject _particleFire = null;
    protected override void CellOnHurt()
    {
        // show texture with decal
        if (_cellID == 0) _material.SetTextureOffset("_MainTex", _offset);
    }
    protected override void HurtOnCell()
    {
        if (_cellID == 0)
        {
            // immolate
            _timeRegen = 0f;
            _rateRegen *= -1f;
            // ? predictable time till detonate
            //_healthInst[_cellID] = _healthCell;
            //
            _particleFire.SetActive(true);
        }
    }
}
