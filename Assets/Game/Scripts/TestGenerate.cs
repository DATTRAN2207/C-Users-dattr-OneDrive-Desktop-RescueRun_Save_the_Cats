using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestGenerate : MonoBehaviour
{
    [SerializeField] private Button testGenerate;
    [SerializeField] private TMP_InputField levelInput;
    [SerializeField] private LevelLoader levelLoader;

    private void Awake()
    {
        testGenerate.onClick.AddListener(() =>
        {
            if (int.TryParse(levelInput.text, out int levelIndex))
            {
                levelLoader.ReloadLevel(levelIndex);
            }
            else
            {
                Debug.LogError($"Invalid level input: {levelInput.text}");
            }
        });
    }
}