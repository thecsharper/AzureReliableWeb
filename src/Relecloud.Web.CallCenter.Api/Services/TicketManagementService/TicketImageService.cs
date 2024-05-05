// Copyright (c) Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License.

using Azure.Storage.Blobs;

namespace Relecloud.Web.Api.Services.TicketManagementService
{
    public class TicketImageService : ITicketImageService
    {
        private readonly ILogger<TicketImageService> _logger;
        private readonly BlobContainerClient _blobContainerClient;

        public TicketImageService(IConfiguration configuration, BlobServiceClient blobServiceClient, ILogger<TicketImageService> logger)
        {
            _logger = logger;

            // It is best practice to create Azure SDK clients once and reuse them.
            // https://learn.microsoft.com/azure/storage/blobs/storage-blob-client-management#manage-client-objects
            // https://devblogs.microsoft.com/azure-sdk/lifetime-management-and-thread-safety-guarantees-of-azure-sdk-net-clients/
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(configuration["App:StorageAccount:Container"]);
        }

        public Task<Stream> GetTicketImagesAsync(string imageName)
        {
            try
            {
                _logger.LogInformation("Retrieving image {ImageName} from blob storage container {ContainerName}.", imageName, _blobContainerClient.Name);
                var blobClient = _blobContainerClient.GetBlobClient(imageName);

                return blobClient.OpenReadAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to retrieve image {ImageName} from blob storage container {ContainerName}", imageName, _blobContainerClient.Name);
                return Task.FromResult(Stream.Null);
            }
        }
    }
}
