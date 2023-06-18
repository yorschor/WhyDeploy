using System.Collections;
using WDBase.Models;

namespace WDBase.Collections;

public class WDJobCollection : IEnumerable<DeployJob>
{
    private readonly Dictionary<string, DeployJob> _jobsById = new();
    private readonly Dictionary<string, DeployJob> _jobsByName = new();
    private readonly Dictionary<string, DeployJob> _jobsByShortName = new();


    public IEnumerator<DeployJob> GetEnumerator()
    {
        return _jobsById.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Result Add(DeployJob job)
    {
        if (_jobsByName.ContainsKey(job.Name)) return new ErrorResult($"Job with name '{job.Name}' already exists.");

        if (_jobsByShortName.ContainsKey(job.ShortName))
            return new ErrorResult($"Job with short name '{job.ShortName}' already exists.");

        if (_jobsById.ContainsKey(job.JobId)) return new ErrorResult($"Job with ID '{job.JobId}' already exists.");

        _jobsByName[job.Name] = job;
        _jobsByShortName[job.ShortName] = job;
        _jobsById[job.JobId] = job;

        return new SuccessResult();
    }


    public void Remove(DeployJob job)
    {
        _jobsByName.Remove(job.Name);
        _jobsByShortName.Remove(job.ShortName);
        _jobsById.Remove(job.JobId);
    }

    public void Clear()
    {
        _jobsByName.Clear();
        _jobsByShortName.Clear();
        _jobsById.Clear();
    }

    public bool Contains(string nameOrShortNameOrId)
    {
        return _jobsByName.ContainsKey(nameOrShortNameOrId)
               || _jobsByShortName.ContainsKey(nameOrShortNameOrId)
               || _jobsById.ContainsKey(nameOrShortNameOrId);
    }

    public Result<DeployJob> Get(string nameOrShortNameOrId)
    {
        if (_jobsByName.TryGetValue(nameOrShortNameOrId, out var job) ||
            _jobsByShortName.TryGetValue(nameOrShortNameOrId, out job) ||
            _jobsById.TryGetValue(nameOrShortNameOrId, out job))
            return new SuccessResult<DeployJob>(job);

        return new ErrorResult<DeployJob>($"No job found with name or short name '{nameOrShortNameOrId}'.");
    }
}