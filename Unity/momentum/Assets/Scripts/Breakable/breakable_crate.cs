using UnityEngine;
public class breakable_crate : base_breakable
{
    protected override void CellOnHurt()
    {
        // show texture with decal
        if (_cellID == 0) _material.SetTextureOffset("_MainTex", _offset);
    }
    protected override void HurtOnCell()
    {
        // instant break
        if (_cellID == 0) _healthInst[_cellID] = 0f;
    }
}
