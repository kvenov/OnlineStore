namespace OnlineStore.Common.OutputMessages
{
    public static class ErrorMessages
    {
        public const string EntityImportError = 
                "There were errors while importing {0} entity!";


        public const string ReferencedEntityMissing = 
                "One or more of the referenced entities by the DTO is not present in the DB!";

        public const string EntityInstanceAlreadyExists =
				"One or more of the imported entities were skipped due to existing record with the same data!";

        public const string EntityDataParseError =
				"There was an error while parsing the entity data!";

        public const string FailedToCreateRole =
                "Failed to create role: {0}";

        public const string FailedToCreateUser =
            "Failed to create user: {0}";

        public const string FailedToAssignUserToRole =
            "Failed to assign {0} role to user: {1}";
	}
}
