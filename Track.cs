using System;

namespace Buggy
{
    
    public enum BuggyOrientation { Clockwise, CounterClockwise };
    public enum BuggyMovement { Stopped, 
                                FollowingLine, 
                                TurningRight, 
                                TurningLeft, 
                                Rotating };

    public class Track
    {
        private enum GantryState { Empty, Occupied, Blocked };
        private enum SectionState { Empty, Occupied, Blocked };

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
            public BuggyMovement movement;
            public double speed; //Between 0.0 and 1.0
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
                new Buggy() {orientation = BuggyOrientation.Clockwise, 
                                movement = BuggyMovement.Stopped, 
                                   speed = 0.5 },
                new Buggy() {orientation = BuggyOrientation.Clockwise,
                                movement = BuggyMovement.Stopped,
                                   speed = 0.5 }
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
            if (oldSectionIndex != -1)
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

        public BuggyOrientation getOrientationOfBuggy(int index)
        {
            if (index < 0 || index >= 2)
            {
                return BuggyOrientation.Clockwise;
            }

            return buggies[index].orientation;
        }

        public void setOrientationOfBuggy(int index, BuggyOrientation orientation)
        {
            if (index < 0 || index >= 2)
            {
                return;
            }

            buggies[index].orientation = orientation;
        }

        public BuggyMovement getMovementOfBuggy(int index)
        {
            if (index < 0 || index >= 2)
            {
                return BuggyMovement.Stopped;
            }

            return buggies[index].movement;
        }

        public void setMovementOfBuggy(int index, BuggyMovement movement)
        {
            if (index < 0 || index >= 2)
            {
                return;
            }

            buggies[index].movement = movement;
        }

        public double getSpeedOfBuggy(int index)
        {
            if (index < 0 || index >= 2)
            {
                return -1.0;
            }

            return buggies[index].speed;
        }

        public void setSpeedOfBuggy(int index, double speed)
        {
            if (index < 0 || index >= 2 ||
                speed < 0.0 || speed > 1.0)
            {
                return;
            }

            buggies[index].speed = speed;
        }

        public int getNextSectionForBuggy(int index, bool parking)
        {
            if (index < 0 || index >= 2)
            {
                return -1;
            }
            if (getOrientationOfBuggy(index) == BuggyOrientation.Clockwise)
            {
                if (getSectionForBuggy(index) == 2 ||
                    getSectionForBuggy(index) == 3 ||
                    getGantryForBuggy(index) == 0)
                {
                    return 0;
                }
                else if (getSectionForBuggy(index) == 0 ||
                   getGantryForBuggy(index) == 1)
                {
                    if (parking)
                    {
                        return 3;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else if (getSectionForBuggy(index) == 1 ||
                   getGantryForBuggy(index) == 2)
                {
                    return 2;
                }
            }
            else
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
            if (getOrientationOfBuggy(index) == BuggyOrientation.Clockwise)
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
                }
                else if (getSectionForBuggy(index) == 1)
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

        private String getCharacterForBuggyIndex(int index)
        {
            if (index == 0)
            {
                return "1";
            }
            else if (index == 1)
            {
                return "2";
            }
            else
            {
                return " ";
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
            map = map.Replace("A", getCharacterForBuggyIndex(getBuggyAtSection(0)));
            map = map.Replace("B", getCharacterForBuggyIndex(getBuggyAtGantry(0)));
            map = map.Replace("C", getCharacterForBuggyIndex(getBuggyAtGantry(1)));
            map = map.Replace("D", getCharacterForBuggyIndex(getBuggyAtSection(3)));
            map = map.Replace("E", getCharacterForBuggyIndex(getBuggyAtSection(2)));
            map = map.Replace("F", getCharacterForBuggyIndex(getBuggyAtGantry(2)));
            map = map.Replace("G", getCharacterForBuggyIndex(getBuggyAtSection(1)));
            return map;
        }
    }
}
