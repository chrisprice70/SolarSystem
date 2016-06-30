﻿/* Copyright 2009 Ivan Krivyakov

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */
using System;
using System.ComponentModel;
using System.Windows.Threading;

namespace SolarSystem
{
    class OrbitsCalculator : INotifyPropertyChanged
    {
        private DateTime _startTime;
        private double _startDays;
        private DispatcherTimer _timer;

        const double EarthYear = 365.25;
        const double EarthRotationPeriod = 1.0;
        const double MarsRotationPeriod = 1.0;
        const double SunRotationPeriod = 25.0;
        const double MartianYear = 686.98;
        const double JupiterYear = 4380;
        const double JupiterRotationPeriod = 0.5;

        const double TwoPi = Math.PI * 2;

        private double _daysPerSecond = 2;
        public double DaysPerSecond
        {
            get { return _daysPerSecond; }
            set { _daysPerSecond = value; Update("DaysPerSecond"); }
        }

        public double EarthOrbitRadius { get { return 80; } set { } }
        public double MarsOrbitRadius { get { return EarthOrbitRadius * 2.4; } set { } }
        public double JupiterOrbitRadius { get { return EarthOrbitRadius * 5.2; } set { } }
        public double Days { get; set; }
        public double EarthRotationAngle { get; set; }
        public double MarsRotationAngle { get; set; }
        public double SunRotationAngle { get; set; }
        public double EarthOrbitPositionX { get; set; }
        public double EarthOrbitPositionY { get; set; }
        public double EarthOrbitPositionZ { get; set; }
        public bool ReverseTime { get; set; }
        public bool Paused { get; set; }
        public double JupiterRotationAngle { get; set; }

        //Mars
        public double MarsOrbitPositionX { get; set; }
        public double MarsOrbitPositionY { get; set; }
        public double MarsOrbitPositionZ { get; set; }

        public double JupiterOrbitPositionX { get; set; }
        public double JupiterOrbitPositionY { get; set; }
        public double JupiterOrbitPositionZ { get; set; }
        public OrbitsCalculator()
        {
            EarthOrbitPositionX = EarthOrbitRadius;
            DaysPerSecond = 2;
        }

        public void StartTimer()
        {
            _startTime = DateTime.Now;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += new EventHandler(OnTimerTick);
            _timer.Start();
        }

        private void StopTimer()
        {
            _timer.Stop();
            _timer.Tick -= OnTimerTick;
            _timer = null;
        }

        public void Pause(bool doPause)
        {
            if (doPause)
            {
                StopTimer();
            }
            else
            {
                StartTimer();
            }
        }

        void OnTimerTick(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            Days += (now-_startTime).TotalMilliseconds * DaysPerSecond / 1000.0 * (ReverseTime?-1:1);
            _startTime = now;
            Update("Days");
            OnTimeChanged();
        }

        private void OnTimeChanged()
        {
            EarthPosition();
            EarthRotation();
            MarsPosition();
            MarsRotation();
            SunRotation();
            JupiterPosition();
            JupiterRotation();
        }

        private void MarsPosition()
        {
            double angle = 2 * Math.PI * Days / MartianYear;
            MarsOrbitPositionX = MarsOrbitRadius * Math.Cos(angle);
            MarsOrbitPositionY = MarsOrbitRadius * Math.Sin(angle);
            Update("MarsOrbitPositionX");
            Update("MarsOrbitPositionY");
        }
        private void JupiterPosition()
        {
            double angle = 2 * Math.PI * Days / JupiterYear;
            JupiterOrbitPositionX = JupiterOrbitRadius * Math.Cos(angle);
            JupiterOrbitPositionY = JupiterOrbitRadius * Math.Sin(angle);
            Update("JupiterOrbitPositionX");
            Update("JupiterOrbitPositionY");
        }
        private void EarthPosition()
        {
            double angle = 2 * Math.PI * Days / EarthYear;
            EarthOrbitPositionX = EarthOrbitRadius * Math.Cos(angle);
            EarthOrbitPositionY = EarthOrbitRadius * Math.Sin(angle);
            Update("EarthOrbitPositionX");
            Update("EarthOrbitPositionY");
        }

        private void MarsRotation()
        {
            MarsRotationAngle = 360 * Days / MarsRotationPeriod;
            Update("MarsRotationAngle");
        }

        private void EarthRotation()
        {
            EarthRotationAngle = 360 * Days / EarthRotationPeriod;
            Update("EarthRotationAngle");
        }
        private void JupiterRotation()
        {
            JupiterRotationAngle = 360 * Days / JupiterRotationPeriod;
            Update("JupiterRotationAngle");
        }

        private void SunRotation()
        {
            SunRotationAngle = 360 * Days / SunRotationPeriod;
            Update("SunRotationAngle");
        }

        private void Update(string propertyName)
        {
            if (PropertyChanged != null)
            {
                var args = new PropertyChangedEventArgs(propertyName);
                PropertyChanged(this, args);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
