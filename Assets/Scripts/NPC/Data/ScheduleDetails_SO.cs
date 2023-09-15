using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScheduleDataList_SO", menuName = "NPCScheule/ScheduleDataList")]
public class ScheduleDetails_SO : ScriptableObject
{
    public List<ScheduleDetails> scheduleDetails;
}
