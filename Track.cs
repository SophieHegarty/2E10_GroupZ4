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
    public void setGantryForBuggy(int buggyIndex, int gantryIndex)
    {
        if (buggyIndex < 0 || buggyIndex >= 2 ||
            gantryIndex < 0 || gantryIndex >= 3)
        {
            return;
        }
        int oldGantryIndex = getGantryForBuggy(buggyIndex);
        int oldSectionIndex = getSectionForBuggy(buggyIndex);
        if (oldGantryIndex != -1)
        {
            gantries[oldGantryIndex].state = GantryState.Empty;
            gantries[oldGantryIndex].buggy = -1;
        }
        if (oldSectionIndex!= -1)
        {
            sections[oldSectionIndex].state = SectionState.Empty;
            sections[oldSectionIndex].buggy = -1;
        }

        gantries[gantryIndex].state = GantryState.Occupied;
        gantries[gantryIndex].buggy = buggyIndex;
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
    public void setSectionForBuggy(int buggyIndex, int sectionIndex)
    {
        if (buggyIndex < 0 || buggyIndex >= 2 ||
            sectionIndex < 0 || sectionIndex >= 4)
        {
            return;
        }
        int oldGantryIndex = getGantryForBuggy(buggyIndex);
        int oldSectionIndex = getSectionForBuggy(buggyIndex);
        if (oldGantryIndex != -1)
        {
            gantries[oldGantryIndex].state = GantryState.Empty;
            gantries[oldGantryIndex].buggy = -1;
        }
        if (oldSectionIndex != -1)
        {
            sections[oldSectionIndex].state = SectionState.Empty;
            sections[oldSectionIndex].buggy = -1;
        }

        sections[sectionIndex].state = SectionState.Occupied;
        sections[sectionIndex].buggy = buggyIndex;
    }
    public String getMap()
    {
        String map = "                     \n"
                   + " .--------A--------. \n"
                   + " |                 | \n"
                   + " B                 C \n"
                   + " |  .-----D-----.  | \n"
                   + " | /             \\ | \n"
                   + " .---E----F----G---. \n"
                   + "                     \n"; 

    } 
}
