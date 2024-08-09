using Google.Protobuf;
using Grpc.Net.Client;
using gRPC_Upload_Client.Protos;

var channel = GrpcChannel.ForAddress("http://localhost:5263");
var client = new FileService.FileServiceClient(channel);

string file = @"/Users/dilanozer/Downloads/beceri.mp4";

using FileStream fileStream = new FileStream(file, FileMode.Open);
var content = new BytesContent
{
    FileSize = fileStream.Length,
    ReadedByte = 0,
    Info = new gRPC_Upload_Client.Protos.FileInfo {
        FileName = Path.GetFileNameWithoutExtension(fileStream.Name),
        FileExtension = Path.GetExtension(fileStream.Name)}
};

var upload = client.FileUpload();
byte[] buffer = new byte[2048];
while ((content.ReadedByte = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
{
    content.Buffer = ByteString.CopyFrom(buffer);
    await upload.RequestStream.WriteAsync(content);
}

await upload.RequestStream.CompleteAsync();
fileStream.Close();

