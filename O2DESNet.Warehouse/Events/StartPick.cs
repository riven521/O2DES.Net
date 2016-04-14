﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using O2DESNet.Warehouse.Dynamics;

namespace O2DESNet.Warehouse.Events
{
    [Serializable]
    internal class StartPick : Event
    {
        internal Picker picker { get; private set; }

        internal StartPick(Simulator sim, Picker picker) : base(sim)
        {
            this.picker = picker;
        }
        public override void Invoke()
        {


            // check start location
            if (picker.CurLocation != _sim.Scenario.StartCP) throw new Exception("Picker not at StartCP, unable to start picking job");

            picker.StartTime = _sim.ClockTime;
            picker.IsIdle = false;
            picker.CompletedJobs.Clear();

            if (_sim.Scenario.MasterPickList[picker.Type].Count > 0)
            {
                picker.Picklist = _sim.Scenario.MasterPickList[picker.Type].First(); // Assign picklist
                picker.Picklist.picker = picker;
                picker.Picklist.startPickTime = _sim.ClockTime;

                _sim.Status.TotalPickListWaitingTime += (_sim.ClockTime - _sim.Status.StartTime);

                picker.PickJobsToComplete = new List<PickJob>(picker.Picklist.pickJobs); // Mutable

                _sim.Scenario.MasterPickList[picker.Type].RemoveAt(0);

                if (picker.PickJobsToComplete.Count > 0)
                {
                    var shelfCP = picker.PickJobsToComplete.First().rack.OnShelf.BaseCP;
                    var duration = picker.GetTravelTime(_sim.Scenario, shelfCP);
                    _sim.ScheduleEvent(new ArriveLocation(_sim, picker), _sim.ClockTime.Add(duration));

                    // Any status updates?
                    _sim.Status.IncrementActivePicker();
                }
                else
                {
                    // Picklist empty
                    _sim.ScheduleEvent(new EndPick(_sim, picker), _sim.ClockTime);
                }
            }
        }

        public override void Backtrack()
        {
            throw new NotImplementedException();
        }
    }
}