# SpacyDotNet

SpacyDotNet is a .NET wrapper for the popular natural language library [spaCy](https://spacy.io/)

## Project scope and limitations

This is not meant to be a complete and exhaustive implementation of all spaCy features and [APIs](https://spacy.io/api). Altough should be enough for basic tasks, it's considered as a starting point if you need to build a complex project using spaCy in .NET 

Most of the basic features in _Spacy101 section_ of the project docs are available. All **Containers** classes are present (_Doc_, _Token_, _Span_ and _Lexeme_) with their basic properties/methods running and also _Vocab_ and _StringStore_ in a limited form.

Furthermore, any developer should be ready to add the missing properties or classes in a very straightforward manner.

## Setup

This projects relies on [Python.NET](http://pythonnet.github.io/) to interop with spaCy, which is written in Python/Cython.

It's been tested under **Windows 10** and **Ubuntu Linux 20.04**. Furthermore, dependency versions are shown below

- .NET Core 3.1 / .NET Standard 2.0
- spaCy 2.3.5
- Python 3.7
- Python.NET 3.0.3 (NuGet: [LostTech.Python.Runtime](https://www.nuget.org/packages/LostTech.Python.Runtime/))

Depending on the dependency, it will likely work with other versions:

- It should work with .NET Core 3.0, but .NET 5.0 is a major release and I can`t really tell.
- It should work with any other spaCy version that changes only its minor/patch version number
- Python.NET 3.0.3 has been compiled against Python 3.7 so the virtual environment must run under this version. In general we should honor the specified Python.NET compiled CPython version

### 1) Create a virtual environment and install spaCy

It's advised to create a virtual environment to install spaCy. Depending on the host system this is done in different ways.

The spaCy official [installation guide](https://spacy.io/usage) is fine, but keep in mind Python 3.7 restriction.

### 2) Run the examples

The provided example expect a command line parameter, that points to the path where the virtual environment lives

If using the CLI to run .NET, (Linux), we should simply browse to _Test/cs_ folder and:

- First, compile the project with `dotnet build` 
- Then, run the example with `dotnet run --venv <path_to_virtualenv>'`

If using Visual Studio, just load _Test.sln_ file, set _--venv <path_to_virtualenv>_ argument in project properties and run the solution.

## Code comparison

I've tried to mimic spaCy API as much as possible, considering the different nature of both C# and Python languages

### C# SpacyDotNet code

```c#
var nlp = spacy.Load("en_core_web_sm");
var doc = nlp.GetDocument("Apple is looking at buying U.K. startup for $1 billion");

foreach (Token token in doc.Tokens)
    Console.WriteLine($"{token.Text} {token.Lemma} {token.PoS} {token.Tag} {token.Dep} {token.Shape} {token.IsAlpha} {token.IsStop}");

Console.WriteLine("");
foreach (Span ent in doc.Ents)
    Console.WriteLine($"{ent.Text} {ent.StartChar} {ent.EndChar} {ent.Label}");

nlp = spacy.Load("en_core_web_md");
var tokens = nlp.GetDocument("dog cat banana afskfsd");

Console.WriteLine("");
foreach (Token token in tokens.Tokens)
    Console.WriteLine($"{token.Text} {token.HasVector} {token.VectorNorm}, {token.IsOov}");

tokens = nlp.GetDocument("dog cat banana");
Console.WriteLine("");
foreach (Token token1 in tokens.Tokens)
{
    foreach (Token token2 in tokens.Tokens)
        Console.WriteLine($"{token1.Text} {token2.Text} {token1.Similarity(token2) }");
}

doc = nlp.GetDocument("I love coffee");
Console.WriteLine("");
Console.WriteLine(doc.Vocab.Strings["coffee"]);
Console.WriteLine(doc.Vocab.Strings[3197928453018144401]);

Console.WriteLine("");
foreach (Token word in doc.Tokens)
{
    var lexeme = doc.Vocab[word.Text];
    Console.WriteLine($@"{lexeme.Text} {lexeme.Orth} {lexeme.Shape} {lexeme.Prefix} {lexeme.Suffix} 
{lexeme.IsAlpha} {lexeme.IsDigit} {lexeme.IsTitle} {lexeme.Lang}");
}
```
### Python spaCy code

```python
nlp = spacy.load("en_core_web_sm")
doc = nlp("Apple is looking at buying U.K. startup for $1 billion")

for token in doc:
    print(token.text, token.lemma_, token.pos_, token.tag_, token.dep_,
          token.shape_, token.is_alpha, token.is_stop)

print("")
for ent in doc.ents:
    print(ent.text, ent.start_char, ent.end_char, ent.label_)

nlp = spacy.load("en_core_web_md")
tokens = nlp("dog cat banana afskfsd")

print("")
for token in tokens:
    print(token.text, token.has_vector, token.vector_norm, token.is_oov)

tokens = nlp("dog cat banana")
print("")
for token1 in tokens:
    for token2 in tokens:
        print(token1.text, token2.text, token1.similarity(token2))

doc = nlp("I love coffee")
print("")
print(doc.vocab.strings["coffee"])  # 3197928453018144401
print(doc.vocab.strings[3197928453018144401])  # 'coffee'

print("")
for word in doc:
    lexeme = doc.vocab[word.text]
    print(lexeme.text, lexeme.orth, lexeme.shape_, lexeme.prefix_, lexeme.suffix_,
            lexeme.is_alpha, lexeme.is_digit, lexeme.is_title, lexeme.lang_)
```
