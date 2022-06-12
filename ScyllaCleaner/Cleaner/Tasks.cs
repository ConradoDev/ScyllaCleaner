using Spotify.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spotify.Cleaner {
    internal class Tasks {

#region Properties
        private static Tuple<Action, string> CurrentTask {
            get {
                return _currentTask;
            }
            set {
                _currentTask = value;
                Console.WriteLine("\n");
                Logger.Log("CurrentTask => " + CurrentTask.Item2, Enums.LogLevel.Warn);
            }
        }
#endregion

        private static int _lastPerformMaximum = 0;
        private static Tuple<Action, string> _currentTask;
        private static Queue<Tuple<Action, string>> AllTasks = new Queue<Tuple<Action, string>>();

        /// <summary>
        /// Perform All Queued Actions.
        /// </summary>
        public static void PerformActions() {
            if (AllTasks.Count < 1)
                throw new ArgumentException("A lista era vazia.");

            _lastPerformMaximum = TasksInQueue();

            while (AllTasks.Count > 0) {
                CurrentTask = AllTasks.FirstOrDefault();

                Task.Run(() => {
                    try { CurrentTask.Item1(); } catch { };
                }).Wait();

                AllTasks.Dequeue();
            }

            Console.Clear();
            Logger.Log("All Actions Performed Sucessfully. Bye bye!", Enums.LogLevel.Success);
            _currentTask = null;
        }

        /// <summary>
        /// Append an action to the queue.
        /// </summary>
        /// <param name="action">The given action.</param>
        public static void AddAction(Action action, string actionName) {
            AllTasks.Enqueue(new Tuple<Action, string>(action, actionName));
            //Logger.Log("TaskAdded => " + AllTasks.Last().Item2, Enums.LogLevel.Neutral);
        }

        /// <summary>
        /// Returns current Task in action.
        /// </summary>
        public static Tuple<Action, string> ActiveTask() {
            return _currentTask;
        }

        /// <summary>
        /// The number of queued tasks.
        /// </summary>
        public static int TasksInQueue() {
            return AllTasks.Count;
        }

        public static int LastPerformanceMaximum() {
            return _lastPerformMaximum;
        }
    }
}