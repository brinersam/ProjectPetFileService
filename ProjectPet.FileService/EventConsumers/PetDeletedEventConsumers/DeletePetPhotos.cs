using MassTransit;
using ProjectPet.FileService.Infrastructure.AmazonS3;
using ProjectPet.VolunteerModule.Contracts.Events;

namespace ProjectPet.FileService.EventConsumers.PetDeletedEventConsumers;

public class DeletePetPhotos : IConsumer<PetDeletedEvent>
{
    private readonly IS3Provider _s3Provider;
    private readonly ILogger<DeletePetPhotos> _logger;

    public DeletePetPhotos(
        IS3Provider s3Provider,
        ILogger<DeletePetPhotos> logger
        )
    {
        _s3Provider = s3Provider;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PetDeletedEvent> context)
    {
        var petId = context.Message.PetId;
        var photosToDelete = context.Message.FileLocations;

        await Task.WhenAll(
                photosToDelete.Select(async x =>
                {
                    var deleteResult = await _s3Provider.DeleteFileAsync(x, CancellationToken.None);
                    if (deleteResult.IsFailure)
                    {
                        _logger.LogWarning(
                            "Failed to delete photo (id: {photoId}) that was owned by now deleted pet (id: {petId}): {error}",
                            x.FileId,
                            petId,
                            deleteResult.Error.Message
                        );
                    }

                    return deleteResult;
                }
                )
        );
    }
}
