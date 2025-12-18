using TMPro;
using UnityEngine;

public class PrototypeTester : MonoBehaviour
{
    [SerializeField] TMP_InputField input;
    [SerializeField] Agent agent;

    public void UpdateState()
    {
        string s = input.text;

        int[] state = new int[9];
        for (int i = 0; i < 9; ++i)
        {
            state[i] = s[i] - '0';
        }

        int[] next = agent.Move(state);

        char[] chars = new char[9];
        for (int i = 0; i < 9; ++i)
        {
            chars[i] = (char)('0' + next[i]);
        }

        input.text = new string(chars);
    }
}
