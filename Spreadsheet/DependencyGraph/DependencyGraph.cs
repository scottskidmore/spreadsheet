// Skeleton implementation by: Joe Zachary, Daniel Kopta, Travis Martin for CS 3500
// Last updated: August 2023 (small tweak to API)
using System.Collections.Generic;
namespace SpreadsheetUtilities;

/// <summary>
/// (s1,t1) is an ordered pair of strings
/// t1 depends on s1; s1 must be evaluated before t1
/// 
/// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
/// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
/// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
/// set, and the element is already in the set, the set remains unchanged.
/// 
/// Given a DependencyGraph DG:
/// 
///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
///        (The set of things that depend on s)    
///        
///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
///        (The set of things that s depends on) 
//
// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
//     dependents("a") = {"b", "c"}
//     dependents("b") = {"d"}
//     dependents("c") = {}
//     dependents("d") = {"d"}
//     dependees("a") = {}
//     dependees("b") = {"a"}
//     dependees("c") = {"a"}
//     dependees("d") = {"b", "d"}
/// </summary>
public class DependencyGraph
{
    private int pairs;
    private Dictionary<string, List<List<string>>> graph;
    
    /// <summary>
    /// Creates an empty DependencyGraph.
    /// </summary>
    public DependencyGraph()
    {
        pairs = 0;
        graph = new Dictionary<string, List<List<string>>>();
    }


    /// <summary>
    /// The number of ordered pairs in the DependencyGraph.
    /// This is an example of a property.
    /// </summary>
    public int NumDependencies
    {
        get { return pairs; }
    }


    /// <summary>
    /// Returns the size of dependees(s),
    /// that is, the number of things that s depends on.
    /// </summary>
    public int NumDependees(string s)
    {
        return graph[s][0].Count;
    }


    /// <summary>
    /// Reports whether dependents(s) is non-empty.
    /// </summary>
    public bool HasDependents(string s)
    {
        if (graph.TryGetValue(s, out _))
        {
            if (graph[s][1].Count > 0)
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Reports whether dependees(s) is non-empty.
    /// </summary>
    public bool HasDependees(string s)
    {
        if (graph.TryGetValue(s, out _))
        {
            if (graph[s][0].Count > 0)
            {
                return true;
            }
        }
        return false;
    }


    /// <summary>
    /// Enumerates dependents(s).
    /// </summary>
    public IEnumerable<string> GetDependents(string s)
    {
        List<List<string>> ?arrays;
        if (graph.TryGetValue(s, out arrays))
        {
       
            return arrays[1];
        }
        return new List<string>();
    }


    /// <summary>
    /// Enumerates dependees(s).
    /// </summary>
    public IEnumerable<string> GetDependees(string s)
    {
        List<List<string>> ?arrays;
        if (graph.TryGetValue(s, out arrays))
        {
            
            return arrays[0];
        }
        return new List<string>();
    }


    /// <summary>
    /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
    /// 
    /// <para>This should be thought of as:</para>   
    /// 
    ///   t depends on s
    ///   
    /// </summary>
    /// <param name="s"> s must be evaluated first. T depends on S</param>
    /// <param name="t"> t cannot be evaluated until s is</param>
    public void AddDependency(string s, string t)
    {
        List<List<string>> ?arrays;
        ///A 2D array is used to hold two arrays that store the dependents and the dependees.
        ///Index 0 holds the array for dependees and index 1 holds dependents
        if(graph.TryGetValue(s, out arrays))
        {
            if (!arrays[1].Contains(t)){
                arrays[1].Add(t);
            }
    
        }
        else
        {
            graph.Add(s, new List<List<string>>{new List<string>(), new List<string>()} );
            if (graph.TryGetValue(s, out arrays))
            {
                arrays[1].Add(t);
            }
        }
        if (graph.TryGetValue(t, out arrays))
        {
            if (!arrays[0].Contains(s))
            {
                arrays[0].Add(s);
            }

        }
        else
        {
            graph.Add(t, new List<List<string>> { new List<string>(), new List<string>() });
            if (graph.TryGetValue(t, out arrays))
            {
                arrays[0].Add(s);
            }
        }
        pairs++;

    }


    /// <summary>
    /// Removes the ordered pair (s,t), if it exists
    /// </summary>
    /// <param name="s"></param>
    /// <param name="t"></param>
    public void RemoveDependency(string s, string t)
    {
        List<List<string>> ?arrays;
        if (graph.TryGetValue(s, out arrays))
        {
            arrays[1].Remove(t);

        }
      
        if (graph.TryGetValue(t, out arrays))
        {
            arrays[0].Remove(s);
            pairs--;
        }
       
    }


    /// <summary>
    /// Removes all existing ordered pairs of the form (s,r).  Then, for each
    /// t in newDependents, adds the ordered pair (s,t).
    /// </summary>
    public void ReplaceDependents(string s, IEnumerable<string> newDependents)
    {
        List<List<string>> ?arrays;
        if (graph.TryGetValue(s, out arrays))
        {
            pairs = pairs - arrays[1].Count;
            arrays[1].Clear();

        }
        foreach (string var in newDependents){
            AddDependency(s, var);
        }
    }


    /// <summary>
    /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
    /// t in newDependees, adds the ordered pair (t,s).
    /// </summary>
    public void ReplaceDependees(string s, IEnumerable<string> newDependees)
    {
        List<List<string>> ?arrays;
        if (graph.TryGetValue(s, out arrays))
        {
            pairs = pairs - arrays[0].Count;
            arrays[0].Clear();

        }
        foreach (string var in newDependees)
        {
            AddDependency(var,s);
        }
    }
}