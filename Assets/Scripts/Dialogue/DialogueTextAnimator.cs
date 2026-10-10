using TMPro;
using UnityEngine;

public class DialogueTextAnimator : MonoBehaviour
{
    [SerializeField]
    private TMP_Text dialogueText;

    private DialogueTextEffect currentEffect;
    private float effectStrength;
    private float effectSpeed;

    private void Awake()
    {
        if (dialogueText == null)
        {
            dialogueText = GetComponent<TMP_Text>();
        }

        enabled = false;
    }

    private void Update()
    {
        if (dialogueText == null ||
            currentEffect == DialogueTextEffect.None)
        {
            return;
        }

        dialogueText.ForceMeshUpdate();

        TMP_TextInfo textInfo = dialogueText.textInfo;
        float time = Time.unscaledTime * effectSpeed;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            TMP_CharacterInfo character = textInfo.characterInfo[i];

            if (!character.isVisible)
            {
                continue;
            }

            int materialIndex = character.materialReferenceIndex;
            int vertexIndex = character.vertexIndex;

            Vector3[] vertices =
                textInfo.meshInfo[materialIndex].vertices;

            Vector3 offset = GetCharacterOffset(i, time);

            vertices[vertexIndex] += offset;
            vertices[vertexIndex + 1] += offset;
            vertices[vertexIndex + 2] += offset;
            vertices[vertexIndex + 3] += offset;
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices =
                textInfo.meshInfo[i].vertices;

            dialogueText.UpdateVertexData(
                TMP_VertexDataUpdateFlags.Vertices
            );
        }
    }

    public void SetEffect(
        DialogueTextEffect effect,
        float strength,
        float speed
    )
    {
        currentEffect = effect;
        effectStrength = strength;
        effectSpeed = speed;

        enabled = effect != DialogueTextEffect.None;
    }

    public void ClearEffect()
    {
        currentEffect = DialogueTextEffect.None;
        enabled = false;

        if (dialogueText != null)
        {
            dialogueText.ForceMeshUpdate();
        }
    }

    private Vector3 GetCharacterOffset(
        int characterIndex,
        float time
    )
    {
        if (currentEffect == DialogueTextEffect.Shake)
        {
            float x = Mathf.PerlinNoise(
                time + characterIndex,
                characterIndex
            ) * 2f - 1f;

            float y = Mathf.PerlinNoise(
                characterIndex,
                time + characterIndex
            ) * 2f - 1f;

            return new Vector3(
                x * effectStrength,
                y * effectStrength,
                0f
            );
        }

        if (currentEffect == DialogueTextEffect.Wobble)
        {
            float y = Mathf.Sin(
                time + characterIndex * 0.35f
            ) * effectStrength;

            return new Vector3(0f, y, 0f);
        }

        return Vector3.zero;
    }
}