using System;

namespace Buggy
{
    public class Track
    {
        private enum GantryState { Empty, Occupied };
        private enum SectionState { Empty, Occupied };
        private enum BuggyOrientation { Clockwise, CounterClockwise};
        private struct Gantry
        {
            public GantryState state;
            public int buggy;
        }
        private struct Section
        {
            public SectionState state;
            public int buggy;
        }
        private struct Buggy
        {
            public BuggyOrientation orientation;
           
        }
        private Gantry[] gantries;
        private Section[] sections;
        private Buggy[] buggies;

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
            buggies = new Buggy[]
            {
                new Buggy() {orientation = BuggyOrientation.Clockwise },
                new Buggy() {orientation = BuggyOrientation.Clockwise }
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
            if (index < 0 || index >= 2)
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

        public BuggyOrientation getOrientationForBuggy(int index)
        {
            if (index < 0 || index >= 2)
            {
                return -1;
            }
            return buggies[index].orientation;
        }
        public void setOrientationForBuggy(int index, BuggyOrientation orientation)
        {
            if (index < 0 || index >= 2)
            {
                return;
            }
            buggies[index].orientation = orientation;
        }
        public int getNextSectionForBuggy(int index, bool parking)
        {
            if(index < 0 || index >= 2)
            {
                return -1;
            }
            if(getOrientationForBuggy(index) == BuggyOrientation.Clockwise)
            {
                if(getSectionForBuggy(index) == 2 || 
                    getSectionForBuggy(index) == 3 ||
                    getGantryForBuggy(index) == 0)
                {
                    return 0;
                }else if(getSectionForBuggy(index) == 0 ||
                    getGantryForBuggy(index)  == 1)
                {
                    if (parking)
                    {
                        return 3;
                    } else
                    {
                        return 1;
                    }
                }else if(getSectionForBuggy(index) == 1 || 
                    getGantryForBuggy(index) == 2)
                {
                    return 2;
                }
            }else 
            {
                if (getSectionForBuggy(index) == 1 ||
                   getSectionForBuggy(index) == 3 ||
                   getGantryForBuggy(index) == 1)
                {
                    return 0;
                }
                else if (getSectionForBuggy(index) == 0 ||
                   getGantryForBuggy(index) == 0)
                {
                    if (parking)
                    {
                        return 3;
                    }
                    else
                    {
                        return 2;
                    }
                }
                else if (getSectionForBuggy(index) == 2 ||
                   getGantryForBuggy(index) == 2)
                {
                    return 1;
                }
            }
            return -1;
        }

        public int getNextGantryForBuggy(int index, bool parking)
        {
            if (index < 0 || index >= 2)
            {
                return -1;
            }
            if (getOrientationForBuggy(index) == BuggyOrientation.Clockwise)
            {
                if (getSectionForBuggy(index) == 2 ||
                    getSectionForBuggy(index) == 3 ||
                    getGantryForBuggy(index) == 2)
                {
                    return 0;
                }
                else if (getSectionForBuggy(index) == 0 ||
                   getGantryForBuggy(index) == 0)
                {
                    return 1;
                }
                else if (getGantryForBuggy(index) == 1)
                {
                    if (parking)
                    {
                        return 0;
                    }
                    else
                    {
                        return 2;
                    }
                }else if(getSectionForBuggy(index) == 1)
                {
                    return 2;
                }
            }
            else
            {
                if (getSectionForBuggy(index) == 1 ||
                   getSectionForBuggy(index) == 3 ||
                   getGantryForBuggy(index) == 2)
                {
                    return 1;
                }
                else if (getSectionForBuggy(index) == 0 ||
                   getGantryForBuggy(index) == 1)
                {
                    return 0;
                }
                else if (getGantryForBuggy(index) == 0)
                {
                    if (parking)
                    {
                        return 1;
                    }
                    else
                    {
                        return 2;
                    }
                }
                else if (getSectionForBuggy(index) == 2)
                {
                    return 2;
                }
            }
            return -1;
        }
        private string getCharacterForBuggyIndex(int index ) {
            if(index == 0)
            {
                return '1';
            }else if(index == 1)
            {
                return '2';
            }else
            {
                return ' ';
            }
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
            map = map.Replace('A', getCharacterForBuggyIndex(getBuggyAtSection(0)));
            map = map.Replace('B', getCharacterForBuggyIndex(getBuggyAtGantry(0)));
            map = map.Replace('C', getCharacterForBuggyIndex(getBuggyAtGantry(1)));
            map = map.Replace('D', getCharacterForBuggyIndex(getBuggyAtSection(3)));
            map = map.Replace('E', getCharacterForBuggyIndex(getBuggyAtSection(2)));
            map = map.Replace('F', getCharacterForBuggyIndex(getBuggyAtGantry(2)));
            map = map.Replace('G', getCharacterForBuggyIndex(getBuggyAtSection(1)));
            return map;
        }
    }
}
