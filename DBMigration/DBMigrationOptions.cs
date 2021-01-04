using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMigration
{
    public record DBMigrationOptions(
        string ConnectionString = null
        , string JournalSchemaName = null
        , string JournalTableName = null);
}
