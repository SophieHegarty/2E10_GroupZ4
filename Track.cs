using System;

public class Track
{
    private enum GantryState {Empty, Occupied};
    private enum SectionState { Empty, Occupied };
    private struct Gantry
    {
        GantryState state;
        int buggy;
    }
    private struct Section
    {
        SectionState state;
        int buggy;
    }
    private Gantry[] gantries;
    private Section[] sections;

	public Track()
	{
        gantries = new Gantry[]
        {
            new Gantry() {state = GantryState.Empty, buggy = -1},
            new Gantry() {state = GantryState.Empty, buggy = -1},
            new Gantry() {state = GantryState.Empty, buggy = -1}
        };
        sections = new Section[]
        {
            new Section() {state = SectionState.Empty, buggy = -1},
            new Section() {state = SectionState.Empty, buggy = -1},
            new Section() {state = SectionState.Empty, buggy = -1},
            new Section() {state = SectionState.Empty, buggy = -1}
        };
	}

    public bool isGantryEmpty(int index)
    {
        if (index < 0 || index >= 3)
        {
            return true;
        }
        return gantries[index].state == GantryState.Empty;
    }
    public int getBuggyAtGantry(int index)
    {
        if (index < 0 || index >= 3)
        {
            return -1;
        }
        return gantries[index].buggy;
    }
    public int getGantryForBuggy(int index)
    {
        if (index < 0|| index >=2)
        {
            return -1;
        }
        for (int i = 0; i < 3; i++)
        {
            if (gantries[i].buggy == index)
            {
                return i;
            }

        }
        return -1;
    }

    public bool isSectionEmpty(int index)
    {
        if (index < 0 || index >= 4)
        {
            return true;
        }
        return sections[index].state == SectionState.Empty;
    }
    public int getBuggyAtSection(int index)
    {
        if (index < 0 || index >= 4)
        {
            return -1;
        }
        return sections[index].buggy;
    }
    public int getSectionForBuggy(int index)
    {
        if (index < 0 || index >= 2)
        {
            return -1;
        }
        for (int i = 0; i < 4; i++)
        {
            if (sections[i].buggy == index)
            {
                return i;
            }

        }
        return -1;
    }
}
