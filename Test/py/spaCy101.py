import spacy
from spacy.tokens import Doc
from spacy.vocab import Vocab

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