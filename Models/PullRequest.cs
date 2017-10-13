using System;
using System.Collections.Generic;

namespace VSTSCore.Models
{
    public class PullRequest
    {
        public int pullRequestId;
        public int codeReviewId;
        public string status;
        public Identity createdBy; 
        public DateTime creationDate;
        public string title;
        public string description;
        public string sourceRefName;
        public string targetRefName;
        public string mergeStatus;
        public Guid mergeId;
        public Commit lastMergeSourceCommit;
        public Commit lastMergeTargetCommit;
        public Commit lastMergeCommit;
        public List<Identity> reviewers;
        public string url;
        public bool supportsIterations;
    }
}