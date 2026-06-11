using System.Collections.Generic;
using UnityEngine;

public class PropRepository : Singleton<PropRepository>
{
    PropCollection propCollectionReal;
    Dictionary<string, GameObject> props;

    private void Awake()
    {
        propCollectionReal = Resources.Load<GameObject>("PropCollection").GetComponent<PropCollection>();
        props = new Dictionary<string, GameObject>();

        foreach (GameObject prop in propCollectionReal.props)
        {
            props[prop.name] = prop;
        }
    }

    public GameObject GetProp(string propName)
    {
        return props[propName];
    }

    public IReadOnlyList<GameObject> GetProps()
    {
        return propCollectionReal.props.AsReadOnly();
    }
}
