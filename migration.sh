cd api
echo "====[Rodando migação test]===="
DATABASE_URL_MINIMAL_API="Server=localhost;Database=desafio21dias_dotnet7_test;Uid=root;Pwd=root" dotnet ef database update # create base de test
echo "====[Rodando migação dev/prod]===="
DATABASE_URL_MINIMAL_API="Server=localhost;Database=desafio21dias_dotnet7;Uid=root;Pwd=root" dotnet ef database update # create base de dev ou prod
