using i5.VIAProMa.DataModel.API;
using System.Collections.Generic;
using UnityEngine;

namespace i5.VIAProMa.WebConnection
{
    /// <summary>
    /// Cache for loaded issues
    /// Stores already loaded issues in order to save network traffic and calls to the API
    /// </summary>
    public static class IssueCache
    {
        /// <summary>
        /// The last time (in seconds since app startup) that an issue was added to the cache
        /// </summary>
        public static float LastTimeUpdated { get; private set; }

        /// <summary>
        /// Time in seconds which specifies how long an issue in the cache stays valid
        /// </summary>
        public static float CacheValidTime { get; set; } = 60f;

        /// <summary>
        /// List of cached issues
        /// </summary>
        private static List<Issue> cachedIssues = new List<Issue>();
        /// <summary>
        /// List of time stamps (in seconds since app startup) when each issue was cached
        /// </summary>
        private static List<float> issueTimeDates = new List<float>();

        /// <summary>
        /// List of indices of the issues which are marked for garbage collection
        /// </summary>
        private static List<int> removeIndices = new List<int>();

        /// <summary>
        /// Adds an issue to the cache
        /// </summary>
        /// <param name="issue">The issue to add to the cache</param>
        public static void AddIssue(Issue issue)
        {
            AssertCacheValid();
            cachedIssues.Add(issue);
            issueTimeDates.Add(Time.time);
            LastTimeUpdated = Time.time;
            AssertCacheValid();
        }

        /// <summary>
        /// Gets a requirement from the Requirements Bazaar
        /// </summary>
        /// <param name="requirementId">The id of the requirement</param>
        /// <returns>Returns the requirement if it was valid and cached or returns null</returns>
        public static Issue GetRequirement(int requirementId)
        {
            // make sure that the cache is valid
            if (Time.time >= LastTimeUpdated + CacheValidTime || !AssertCacheValid())
            {
                // cache was not valid: now the cache is empty, so nothing to find
                return null;
            }

            // search all issues to find the given one
            for (int i = 0; i < cachedIssues.Count; i++)
            {
                // while searching, we can also check if each item is still valid
                bool valid = Time.time <= issueTimeDates[i] + CacheValidTime;
                if (!valid)
                {
                    removeIndices.Add(i); // mark for garbage collection
                }
                if (cachedIssues[i].Source == DataSource.REQUIREMENTS_BAZAAR && cachedIssues[i].Id == requirementId)
                {
                    // have found the issue
                    if (valid)
                    {
                        // first get the issue because the index can change after garbage collection
                        Issue foundIssue = cachedIssues[i];
                        // garbage collect now since we will return directly afterwards
                        GarbageCollect();
                        return foundIssue;
                    }
                    else
                    {
                        // garbage collect now since we will return directly afterwards
                        GarbageCollect();
                        return null;
                    }
                }
            }
            // have not found the issue
            // garbage collect now and return null
            GarbageCollect();
            return null;
        }

        /// <summary>
        /// Gets a GitHub issue
        /// </summary>
        /// <param name="repositoryId">The Id of the GitHub repository which contains the issue</param>
        /// <param name="issueNumber">The number of the issue in the repository</param>
        /// <returns>The GitHub issue if it was valid and cached or returns null</returns>
        public static Issue GetGitHubIssue(int repositoryId, int issueNumber)
        {
            // make sure that the cache is valid
            if (Time.time >= LastTimeUpdated + CacheValidTime || !AssertCacheValid())
            {
                // cache was not valid: now the cache is empty, so nothing to find
                return null;
            }

            for (int i = 0; i < cachedIssues.Count; i++)
            {
                bool valid = Time.time <= issueTimeDates[i] + CacheValidTime;
                if (!valid)
                {
                    removeIndices.Add(i); // mark for garbage collection
                }
                if (cachedIssues[i].Source == DataSource.GITHUB && cachedIssues[i].ProjectId == repositoryId && cachedIssues[i].Id == issueNumber)
                {
                    // have found the issue
                    // garbage collect now since we will return directly afterwards
                    GarbageCollect();
                    if (valid)
                    {
                        return cachedIssues[i];
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            // have not found the issue
            // garbage collect now and return null
            GarbageCollect();
            return null;
        }

        /// <summary>
        /// Deletes cached issues which are longer in the cache than CacheValidTime
        /// </summary>
        private static void GarbageCollect()
        {
            AssertCacheValid();
            for (int i = cachedIssues.Count - 1; i >= 0; i--)
            {
                if (removeIndices.Contains(i))
                {
                    cachedIssues.RemoveAt(i);
                    cachedIssues.RemoveAt(i);
                }
            }
            removeIndices.Clear();
            AssertCacheValid();
        }

        /// <summary>
        /// Checks that the cache is still valid, i.e. that the two arrays of cachedIssues and issueTimeDates have the same length
        /// If not, it logs an erro and resets the cache
        /// </summary>
        /// <returns>True if the cache is valid, otherwise false</returns>
        private static bool AssertCacheValid()
        {
            if (cachedIssues.Count != issueTimeDates.Count)
            {
                Debug.LogError("The issue cache has different length arrays for issues and time dates (" + cachedIssues.Count + " vs. " + issueTimeDates.Count
                    + "\nClearing the cache to return to a correct state.");
                cachedIssues.Clear();
                issueTimeDates.Clear();
                removeIndices.Clear();
                return false;
            }
            return true;
        }
    }
}