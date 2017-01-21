using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Ground : MonoBehaviour
{

    private float _timeCounter;
    private PaletteSwapper _swapper;
    // Use this for initialization
    void Start()
    {
        _swapper = GetComponent<PaletteSwapper>();
    }

    // Update is called once per frame
    void Update()
    {
        _timeCounter -= Time.deltaTime;
        if (_timeCounter <= 0)
        {
            int index;
            do
            {
                index = Random.Range(0, _swapper.Palettes.Length);
            } while (index == _swapper.CurrentPaletteIndex);
            _swapper.CurrentPaletteIndex = index;
            _timeCounter = .25f;
        }
    }
}
