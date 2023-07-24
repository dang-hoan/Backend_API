﻿using Application.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.FeedbackFileUpload
{
    public interface IFeedbackFileUploadRepository : IRepositoryAsync<Domain.Entities.FeebackFileUpload.FeedbackFileUpload,long>
    {
    }
}