cd api
echo "====[Rodando migração prod]===="
#DATABASE_URL_MINIMAL_API="Server=localhost;Database=desafio21dias_dotnet7_prod;Uid=root;Pwd=root" dotnet ef database update # create base de dev ou prod

dotnet publish -o Release
cp appsettings.Sample.json appsettings.json
export DATABASE_URL_MINIMAL_API="Server=localhost;Database=desafio21dias_dotnet7_prod;Uid=root;Pwd=root"
#nohup dotnet Release/minimal-api-desafio.dll &

dotnet Release/minimal-api-desafio.dll
