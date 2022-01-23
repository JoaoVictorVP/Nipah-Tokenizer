using NipahTokenizer;
using NipahTokenizer.Parsing;

var parser = new Parser();

parser.Fragment("Root")
    .Optional()
        .Match("var")
        .Fragment("variable_declaration")
            .Capture("variable_name")
            .Optional()
                .Match("=")
                .Fragment("variable_value")
                    .Capture("var_value", "Expecting value");

/*parser.Fragment("variable_declaration")
    .Match("var")
    .Capture("var_name")
    .Optional()
        .Match("=")
        .Fragment("variable_value")
            .Capture("var_value");*/
    //Match("=").
    //Capture("var_value");

var tokenizer = new Tokenizer();
while (true)
{
    string code = read("> ");

    var tokens = new Tokens(tokenizer.Tokenize(code));

    var parsedTree = parser.Parse(tokens);

    printf(parsedTree);

    printf("--- Now processed version ---");

    var processed = Processor.MakePostProcess()
        .SearchFor("variable_declaration")
            .SearchFor("variable_name")
                .EmitSelf()
            .End()
            .SearchFor("variable_value")
                .Emit()
            .End()
        .End()
    .Build(parsedTree);

    printf(processed);

    pause();

    clear();
}