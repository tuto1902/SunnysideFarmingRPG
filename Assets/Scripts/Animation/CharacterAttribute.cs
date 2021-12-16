[System.Serializable]

public struct CharacterAttribute
{
    public CharacterPartAnimator characterPart;
    public PartVariantType partVariantType;

    public CharacterAttribute(CharacterPartAnimator characterPart, PartVariantType partVariantType)
    {
        this.characterPart = characterPart;
        this.partVariantType = partVariantType;
    }
}
