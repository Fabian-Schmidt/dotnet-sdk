// ------------------------------------------------------------
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
// ------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;

namespace Samples.Client
{
    public class StateStoreETagsExample : Example
    {
        private static readonly string stateKeyName = "widget";
        private static readonly string storeName = "statestore";

        public override string DisplayName => "Using the State Store with ETags";

        public override async Task RunAsync(CancellationToken cancellationToken)
        {
            var client = new DaprClientBuilder().Build();

            // Save state with an empty etag. This will create an etag for the state entry
            await client.TrySaveStateAsync(storeName, stateKeyName, new Widget() { Size = "small", Color = "yellow", }, null, cancellationToken: cancellationToken);

            // Read the state and etag
            var (original, originalETag) = await client.GetStateAndETagAsync<Widget>(storeName, stateKeyName, cancellationToken: cancellationToken);
            Console.WriteLine($"Retrieved state: {original.Size}  {original.Color} with etag: {originalETag}");

            // Modify the state with a null etag. This will update the etag
            original.Color = "orange";
            await client.TrySaveStateAsync<Widget>(storeName, stateKeyName, original, null, cancellationToken: cancellationToken);
            Console.WriteLine($"Saved modified state : {original.Size}  {original.Color}");

            // Read the updated state and etag
            var (updated, updatedETag) = await client.GetStateAndETagAsync<Widget>(storeName, stateKeyName, cancellationToken: cancellationToken);
            Console.WriteLine($"Retrieved state: {updated.Size}  {updated.Color} with etag: {updatedETag}");

            // Modify the state and try saving it with the old etag. This will fail
            updated.Color = "purple";
            var isSaveStateSuccess = await client.TrySaveStateAsync<Widget>(storeName, stateKeyName, updated, originalETag, cancellationToken: cancellationToken);
            Console.WriteLine($"Saved state with old etag: {originalETag} successfully? {isSaveStateSuccess}");

            // Specify an empty etag. This will throw an ArgumentException
            try
            {
                await client.TrySaveStateAsync<Widget>(storeName, stateKeyName, updated, "", cancellationToken: cancellationToken);
            }
            catch(ArgumentException)
            {
                Console.WriteLine($"TrySaveStateAsync with empty etag threw ArgumentException");
            }

            // Modify the state and try saving it with the updated etag. This will succeed
            isSaveStateSuccess = await client.TrySaveStateAsync<Widget>(storeName, stateKeyName, updated, updatedETag, cancellationToken: cancellationToken);
            Console.WriteLine($"Saved state with latest etag: {updatedETag} successfully? {isSaveStateSuccess}");

            // Delete with an empty etag. This will throw an ArgumentException
            try
            {
                await client.TryDeleteStateAsync(storeName, stateKeyName, "", cancellationToken: cancellationToken);
            }
            catch(ArgumentException)
            {
                Console.WriteLine($"TryDeleteStateAsync with empty etag threw ArgumentException");
            }

            // Delete with an old etag. This will fail
            var isDeleteStateSuccess = await client.TryDeleteStateAsync(storeName, stateKeyName, originalETag, cancellationToken: cancellationToken);
            Console.WriteLine($"Deleted state with old etag: {originalETag} successfully? {isDeleteStateSuccess}");

            // Read the updated state and etag
            (updated, updatedETag) = await client.GetStateAndETagAsync<Widget>(storeName, stateKeyName, cancellationToken: cancellationToken);
            Console.WriteLine($"Retrieved state: {updated.Size}  {updated.Color} with etag: {updatedETag}");

            // Delete with updated etag. This will succeed
            isDeleteStateSuccess = await client.TryDeleteStateAsync(storeName, stateKeyName, updatedETag, cancellationToken: cancellationToken);
            Console.WriteLine($"Deleted state with updated etag: {updatedETag} successfully? {isDeleteStateSuccess}");
        }

        private class Widget
        {
            public string? Size { get; set; }
            public string? Color { get; set; }
        }
    }
}
